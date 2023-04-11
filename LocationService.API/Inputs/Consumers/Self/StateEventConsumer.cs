using System;
using System.Threading.Tasks;
using LocationService.Message.Definition.Events;
using LocationService.Message.Definition.Events.Cache;
using LocationService.Message.Definition.Events.States.v1;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.API.Inputs.Consumers.Self;

public class StateEventConsumer : IConsumer<StateEvent>
{
    private readonly ILogger<StateEventConsumer> _logger;
    private readonly IMediator _mediator;

    public StateEventConsumer(ILogger<StateEventConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<StateEvent> context)
    {
        try
        {
            _logger.LogInformation("Received message of type {MessageType} from {Source} sent on {SentTime}", nameof(StateEvent), context.SourceAddress, context.SentTime.ToString());
            var locationEvent = context.Message;
            switch (locationEvent.Action)
            {
                case EventAction.StateCreate:
                case EventAction.StateUpdate: 
                case EventAction.StateDelete:
                    _logger.LogDebug("Cache key removal triggered by {Event} for id {Id}", nameof(StateEvent), locationEvent.Details.Id);
                    _ = _mediator.Send(new ClearCache
                    {
                        StateId = locationEvent.Details.Id,
                        StateCode = locationEvent.Details.Code,
                        CountryId = locationEvent.Details.CountryId
                    });
                    break;

                default:
                    await Task.CompletedTask;
                    break;
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Cannot consume {Event} event - {Error}",nameof(StateEvent), e.Message);
            throw;
        }
    }
}
