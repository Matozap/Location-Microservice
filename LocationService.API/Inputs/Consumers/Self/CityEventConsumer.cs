using System;
using System.Threading.Tasks;
using LocationService.Message.Events;
using LocationService.Message.Events.Cache;
using LocationService.Message.Events.Cities.v1;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.API.Inputs.Consumers.Self;

public class CityEventConsumer : IConsumer<CityEvent>
{
    private readonly ILogger<CityEventConsumer> _logger;
    private readonly IMediator _mediator;

    public CityEventConsumer(ILogger<CityEventConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<CityEvent> context)
    {
        try
        {
            _logger.LogInformation("Received message of type {MessageType} from {Source} sent on {SentTime}", nameof(CityEvent), context.SourceAddress, context.SentTime.ToString());
            var locationEvent = context.Message;
            switch (locationEvent.Action)
            {
                case EventAction.Created:
                case EventAction.Updated: 
                case EventAction.Deleted: 
                    _logger.LogDebug("Cache key removal triggered by {Event} for id {Id}", nameof(CityEvent), locationEvent.Details.Id);
                    _ = _mediator.Send(new ClearCache
                    {
                        CityId = locationEvent.Details.Id,
                        StateId = locationEvent.Details.StateId
                    });
                    break;

                case EventAction.None:
                default:
                    await Task.CompletedTask;
                    break;
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Cannot consume {Event} event - {Error}",nameof(CityEvent), e.Message);
            throw;
        }
    }
}
