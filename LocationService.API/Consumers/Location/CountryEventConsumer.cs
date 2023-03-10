using System;
using System.Threading.Tasks;
using LocationService.Message.Definition;
using LocationService.Message.Definition.Countries.Events.v1;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocationService.API.Consumers.Location;

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
            _logger.LogInformation("Received message from {Source} sent on {SentTime}", context.SourceAddress, context.SentTime.ToString());
            var locationEvent = context.Message;
            switch (locationEvent.Action)
            {
                case EventAction.CountryCreate:
                case EventAction.CountryUpdate: 
                case EventAction.CountryDelete: 
                    _logger.LogDebug("Cache key removal triggered by {Event} for id {Id}", nameof(CountryEvent), locationEvent.LocationDetails.Id);
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
            _logger.LogError("Cannot consume {Event} event - {Error}",nameof(CountryEvent), e.Message);
            throw;
        }
    }
}
