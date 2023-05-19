using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using LocationService.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace LocationService.API.Helpers.Middleware;

internal sealed class GlobalExceptionMiddleware : Interceptor, IFunctionsWorkerMiddleware, IMiddleware
{
    private record ErrorResponse(int StatusCode, string Errors);
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    
    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleFunctionExceptionAsync(context, ex);
        }
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            await HandleHttpExceptionAsync(context, e);
        }
    }
    
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            throw HandleGrpcExceptionAsync(ex);
        }
    }
    
    private RpcException HandleGrpcExceptionAsync(Exception exception)
    {
        _logger.LogError(exception, "");
        var statusCode = GetStatusCode(exception).grpcCode;
        var errors = GetErrors(exception);
        return new RpcException(new Status(statusCode, errors));
    }
    
    private async Task HandleHttpExceptionAsync(HttpContext httpContext, Exception exception)
    {
        _logger.LogError(exception, "");
        var statusCode = (int)GetStatusCode(exception).httpCode;
        var errors = GetErrors(exception);
        var response = new ErrorResponse(statusCode, errors);
        
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCode;
        
        await httpContext.Response.WriteAsync(response.Serialize());
    }

    private static async Task HandleFunctionExceptionAsync(FunctionContext context, Exception exception)
    {
        var log = context.GetLogger<GlobalExceptionMiddleware>();
        log.LogError(exception, "");
            
        var httpReqData = await context.GetHttpRequestDataAsync();

        if (httpReqData != null)
        {
            var status = GetStatusCode(exception).httpCode;
            var errors = GetErrors(exception);
            var response = new ErrorResponse((int)status, errors);
            var newHttpResponse = httpReqData.CreateResponse(status);
            await newHttpResponse.WriteAsJsonAsync(response, newHttpResponse.StatusCode);

            var invocationResult = context.GetInvocationResult();

            var httpOutputBindingFromMultipleOutputBindings = GetHttpOutputBindingFromMultipleOutputBinding(context);
            if (httpOutputBindingFromMultipleOutputBindings is not null)
            {
                httpOutputBindingFromMultipleOutputBindings.Value = newHttpResponse;
            }
            else
            {
                invocationResult.Value = newHttpResponse;
            }
        }
    }

    private static (HttpStatusCode httpCode, StatusCode grpcCode) GetStatusCode(Exception exception) =>
        exception switch
        {
            ValidationException => (HttpStatusCode.BadRequest, StatusCode.InvalidArgument),
            AggregateException => (HttpStatusCode.BadRequest, StatusCode.InvalidArgument),
            _ => (HttpStatusCode.InternalServerError, StatusCode.Internal)
        };
    
    private static string GetErrors(Exception exception)
    {
        switch (exception)
        {
            case ValidationException validationException:
                return ExtractValidationErrors(validationException);
            case AggregateException aggregateException:
            {
                var message = "";
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    if (innerException is ValidationException validationExceptions)
                    {
                        message += ExtractValidationErrors(validationExceptions);
                    }
                }
                return message;
            }
            default:
                return exception.Message;
        }
    }

    private static string ExtractValidationErrors(ValidationException validationException) => string.Join(", ", validationException.Errors.Select(failure => failure.ErrorMessage).ToArray());

    private static OutputBindingData<HttpResponseData> GetHttpOutputBindingFromMultipleOutputBinding(FunctionContext context) =>
        context.GetOutputBindings<HttpResponseData>()
            .FirstOrDefault(binding => binding.BindingType == "http" && binding.Name != "$return");
}