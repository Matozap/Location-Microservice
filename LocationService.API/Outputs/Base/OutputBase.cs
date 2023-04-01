using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocationService.API.Outputs.Base;

public class OutputBase : ControllerBase
{
    private readonly OutputType _outputType;

    public OutputBase(OutputType outputType)
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
            HttpStatusCode.Created => Created("", value),
            HttpStatusCode.NotFound => NotFound(),
            HttpStatusCode.Conflict => Conflict(),
            HttpStatusCode.NoContent => NoContent(),
            _ => Ok(value)
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
        await Task.CompletedTask;
        return value;
    }
}

public enum OutputType
{
    Controller,
    AzureFunction,
    Grpc
}