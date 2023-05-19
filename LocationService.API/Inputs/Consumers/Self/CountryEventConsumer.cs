using System;
using System.Threading.Tasks;
using LocationService.Message.Events.Countries.v1;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LocationService.API.Inputs.Consumers.Self;

public class CountryEventConsumer : IConsumer<CountryEvent>
{
    private readonly ILogger<CountryEventConsumer> _logger;

    public CountryEventConsumer(ILogger<CountryEventConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<CountryEvent> context)
    {
        var locationEvent = context.Message;
        _logger.LogInformation("Received own message of type {MessageType} with action '{Action}' from {Source} sent on {SentTime}", nameof(CountryEvent), Enum.GetName(locationEvent.Action), context.SourceAddress, context.SentTime.ToString());
        await Task.CompletedTask;
    }
}
