using System;
using System.Threading.Tasks;
using LocationService.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LocationService.Infrastructure.Data.Bus;

public sealed class EventBus : IEventBus
{
    private readonly EventBusOptions _options;
    private readonly ILogger<EventBus> _logger;
    private readonly IBus _bus;
    private readonly IPublishEndpoint _publishEndpoint;

    public EventBus(EventBusOptions options, ILogger<EventBus> logger, IBus bus, IPublishEndpoint publishEndpoint)
    {
        _options = options;
        _options = options;
        _logger = logger;
        _bus = bus;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Publish<T>(T message) where T : class 
    {
        try
        {
            if(_options.Disabled)
                return;
            
            await _publishEndpoint.Publish(message);
            _logger.LogInformation("Publishing changes to {Destination}", message.GetType().Name);
            
            if (_options.SendToAdditionalPath?.Count > 0)
            {
                foreach (var (name,destination) in _options.SendToAdditionalPath)
                {
                    if (!string.IsNullOrEmpty(destination))
                    {
                        var endpoint = await _bus.GetSendEndpoint(new Uri($"topic:{destination}"));
                        await endpoint.Send(message);
                        _logger.LogInformation("Publishing changes to additional topic {Destination}", name);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing changes");
        }
    }
}
