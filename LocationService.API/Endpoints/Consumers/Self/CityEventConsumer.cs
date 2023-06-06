using System;
using System.Threading.Tasks;
using LocationService.Application.Cities.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LocationService.API.Endpoints.Consumers.Self;

public class CityEventConsumer : IConsumer<CityEvent>
{
    private readonly ILogger<CityEventConsumer> _logger;

    public CityEventConsumer(ILogger<CityEventConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<CityEvent> context)
    {
        var locationEvent = context.Message;
        _logger.LogInformation("Received own message of type {MessageType} with action '{Action}' from {Source} sent on {SentTime}", nameof(CityEvent), Enum.GetName(locationEvent.Action), context.SourceAddress, context.SentTime.ToString());
        await Task.CompletedTask;
    }
}
