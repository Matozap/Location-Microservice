using System;
using System.Threading.Tasks;
using LocationService.Message.Events.States.v1;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LocationService.API.Inputs.Consumers.Self;

public class StateEventConsumer : IConsumer<StateEvent>
{
    private readonly ILogger<StateEventConsumer> _logger;

    public StateEventConsumer(ILogger<StateEventConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<StateEvent> context)
    {
        var locationEvent = context.Message;
        _logger.LogInformation("Received own message of type {MessageType} with action '{Action}' from {Source} sent on {SentTime}", nameof(StateEvent), Enum.GetName(locationEvent.Action), context.SourceAddress, context.SentTime.ToString());
        await Task.CompletedTask;
    }
}
