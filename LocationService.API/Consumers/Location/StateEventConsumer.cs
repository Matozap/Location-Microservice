using System;
using System.Threading.Tasks;
using LocationService.Message.Definition;
using LocationService.Message.Definition.States.Events.v1;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.API.Consumers.Location;

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
                    _logger.LogDebug("Cache key removal triggered by {Event} for id {Id}", nameof(StateEvent), locationEvent.LocationDetails.Id);
                    _ = _mediator.Send(new ClearCache
                    {
                        StateId = locationEvent.LocationDetails.Id,
                        StateCode = locationEvent.LocationDetails.Code,
                        CountryId = locationEvent.LocationDetails.CountryId
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
