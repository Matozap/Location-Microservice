using System.Reflection;
using LocationService.Application.Interfaces;
using LocationService.Infrastructure.Data.Queue;
using LocationService.Message.Messaging.Event.v1;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LocationService.API.Extensions;

public static class EventBusConfigurationExtension
{
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        var eventBusOptions = new EventBusOptions();
        configuration.GetSection("EventBus").Bind(eventBusOptions);
        
        if (!eventBusOptions.Disabled)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.SetInMemorySagaRepositoryProvider();
                x.AddConsumers(entryAssembly);
                x.AddSagaStateMachines(entryAssembly);
                x.AddSagas(entryAssembly);
                x.AddActivities(entryAssembly);

                switch (eventBusOptions.UseInMemory)
                {
                    case true:
                        x.UsingInMemory((context, cfg) =>
                        {
                            cfg.Message<CountryEvent>(e => e.SetEntityName(nameof(CountryEvent)));
                            cfg.Message<StateEvent>(e => e.SetEntityName(nameof(StateEvent)));
                            cfg.Message<CityEvent>(e => e.SetEntityName(nameof(CityEvent)));
                            cfg.ConfigureEndpoints(context);
                        });
                        break;
                    
                    default:
                        x.UsingAzureServiceBus((context, cfg) =>
                        {
                            cfg.Host(eventBusOptions.ConnectionString);
                            
                            cfg.Message<CountryEvent>(e => e.SetEntityName(nameof(CountryEvent)));
                            cfg.Message<StateEvent>(e => e.SetEntityName(nameof(StateEvent)));
                            cfg.Message<CityEvent>(e => e.SetEntityName(nameof(CityEvent)));

                            foreach (var (subscriptionKey,subscriptionValue) in eventBusOptions.Subscriptions)
                            {
                                if (!string.IsNullOrEmpty(subscriptionValue))
                                {
                                    cfg.SubscriptionEndpoint(subscriptionValue,eventBusOptions.Destination, configurator =>
                                    {
                                        configurator.ConfigureDeadLetterQueueDeadLetterTransport();
                                        configurator.ConfigureDeadLetterQueueErrorTransport();
                                        configurator.PublishFaults = false;
                                    });
                                }
                            }

                            cfg.ConfigureEndpoints(context);
                        });
                        break;
                }
            });
        }
        
        services.AddSingleton(eventBusOptions);
        services.AddScoped<IEventBus, EventBus>();
        return services;
    } 
}
