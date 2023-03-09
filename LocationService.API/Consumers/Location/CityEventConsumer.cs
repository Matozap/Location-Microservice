using System;
using System.Threading.Tasks;
using LocationService.Message.Definition;
using LocationService.Message.Definition.Cities.Events.v1;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.API.Consumers.Location;

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
            _logger.LogInformation("Received message from {Source} sent on {SentTime} with correlation id {CorrelationId}", context.SourceAddress, context.SentTime.ToString(), context.CorrelationId.ToString());
            var locationEvent = context.Message;
            switch (locationEvent.Action)
            {
                case EventAction.CityCreate:
                case EventAction.CityUpdate : 
                case EventAction.CityDelete : 
                    _logger.LogDebug("Removing cache key for City id {CityId} ", locationEvent.LocationDetails.Id);
                    _ = _mediator.Send(new ClearCache
                    {
                        CityId = locationEvent.LocationDetails.Id,
                        StateId = locationEvent.LocationDetails.StateId
                    });
                    break;

                default:
                    await Task.CompletedTask;
                    break;
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Cannot consume location event - {Error}", e.Message);
            throw;
        }
    }
}