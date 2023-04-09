using System;
using System.Net;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Outputs.Base;

public class OutputBase
{
    private readonly OutputType _outputType;

    protected OutputBase(OutputType outputType)
    {
        _outputType = outputType;
    }

    protected async Task<object> TransformToOutputAsync(object value, HttpStatusCode httpStatusCode, HttpRequestData httpRequestData = null)
    {
        switch (_outputType)
        {
            case OutputType.Controller:
            default:
                return await TransformToControllerOutputAsync(value, httpStatusCode);
            case OutputType.AzureFunction:
                ArgumentNullException.ThrowIfNull(httpRequestData);
                return await TransformToFunctionOutputAsync(value, httpStatusCode, httpRequestData);
            case OutputType.Grpc:
                return await TransformToGrpcOutputAsync(value, httpStatusCode);
        }
    }

    private async Task<IActionResult> TransformToControllerOutputAsync(object value, HttpStatusCode httpStatusCode)
    {
        await Task.CompletedTask;
        return httpStatusCode switch
        {
            HttpStatusCode.Created => new CreatedResult( "", value),
            HttpStatusCode.NotFound => new NotFoundResult(),
            HttpStatusCode.Conflict => new ConflictResult(),
            HttpStatusCode.NoContent => new NoContentResult(),
            _ => new OkObjectResult(value)
        };
    }
    
    private async Task<HttpResponseData> TransformToFunctionOutputAsync(object value, HttpStatusCode httpStatusCode, HttpRequestData httpRequestData)
    {
        var response = httpRequestData.CreateResponse(httpStatusCode);

        if (value != null)
        {
            await response.WriteAsJsonAsync(value);
        }

        return response;
    }
    
    private async Task<object> TransformToGrpcOutputAsync(object value, HttpStatusCode httpStatusCode)
    {
        return httpStatusCode switch
        {
            HttpStatusCode.Conflict => throw new RpcException(new Status(StatusCode.AlreadyExists, "")),
            HttpStatusCode.NotFound => throw new RpcException(new Status(StatusCode.NotFound, "")),
            HttpStatusCode.NoContent => throw new RpcException(new Status(StatusCode.OK, "")),
            _ => await Task.FromResult(value)
        };
    }
}

public enum OutputType
{
    Controller,
    AzureFunction,
    Grpc
}