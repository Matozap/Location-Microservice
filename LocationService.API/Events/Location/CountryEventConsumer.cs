using System;
using System.Threading.Tasks;
using LocationService.Message.Messaging.Event;
using LocationService.Message.Messaging.Event.v1;
using LocationService.Message.Messaging.Request.Cache;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.API.Events.Location;

public class CountryEventConsumer : IConsumer<CountryEvent>
{
    private readonly ILogger<CountryEventConsumer> _logger;
    private readonly IMediator _mediator;

    public CountryEventConsumer(ILogger<CountryEventConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<CountryEvent> context)
    {
        try
        {
            _logger.LogInformation("Received message from {Source} sent on {SentTime} with correlation id {CorrelationId}", context.SourceAddress, context.SentTime.ToString(), context.CorrelationId.ToString());
            var locationEvent = context.Message;
            switch (locationEvent.Action)
            {
                case EventAction.CountryCreate:
                case EventAction.CountryUpdate : 
                case EventAction.CountryDelete : 
                    _logger.LogDebug("Removing cache key for Country id {CountryId} ", locationEvent.LocationDetails.Id);
                    _ = _mediator.Send(new ClearCache
                    {
                        CountryId = locationEvent.LocationDetails.Id
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
