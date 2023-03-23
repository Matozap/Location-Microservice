using System.Net;
using LocationService.Message.Definition.Countries.Requests.v1;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace LocationService.AFX.Functions;

public class CountryFunction
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public CountryFunction(ILoggerFactory loggerFactory, IMediator mediator)
    {
        _mediator = mediator;
        _logger = loggerFactory.CreateLogger<CountryFunction>();
    }
    
    [Function("CountryFunction")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(await _mediator.Send(new GetAllCountries()));
        return response;
    }
}