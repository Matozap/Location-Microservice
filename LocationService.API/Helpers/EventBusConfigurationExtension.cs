using System;
using System.Collections.Generic;
using System.Reflection;
using LocationService.API.Consumers.Location;
using LocationService.Application.Interfaces;
using LocationService.Infrastructure.Bus;
using LocationService.Message.Definition.Cities.Events.v1;
using LocationService.Message.Definition.Countries.Events.v1;
using LocationService.Message.Definition.States.Events.v1;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LocationService.API.Helpers;

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
                x.AddConsumers(entryAssembly);

                switch (eventBusOptions.BusType)
                {
                    default:
                    case "InMemory":
                        x.UsingInMemory((context, cfg) =>
                        {
                            SetEventBusMessages(cfg);
                            cfg.ConfigureEndpoints(context);
                        });
                        break;
                    
                    case "Azure Service Bus":
                        x.UsingAzureServiceBus((context, cfg) =>
                        {
                            cfg.Host(eventBusOptions.ConnectionString);

                            SetEventBusMessages(cfg);
                            RegisterSubscription(context, cfg, eventBusOptions.Subscriptions);
                            cfg.ConfigureEndpoints(context);
                        });
                        break;
                    case "RabbitMQ":
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host(new Uri(eventBusOptions.ConnectionString));

                            SetEventBusMessages(cfg);
                            RegisterSubscriptionOnRabbitMq(context, cfg, eventBusOptions.Subscriptions);
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

    private static void SetEventBusMessages<T>(IBusFactoryConfigurator<T> busFactoryConfigurator) where T : IReceiveEndpointConfigurator
    {
        busFactoryConfigurator.Message<CountryEvent>(e => e.SetEntityName(nameof(CountryEvent)));
        busFactoryConfigurator.Message<StateEvent>(e => e.SetEntityName(nameof(StateEvent)));
        busFactoryConfigurator.Message<CityEvent>(e => e.SetEntityName(nameof(CityEvent)));
    }
    
    private static void RegisterSubscription(IBusRegistrationContext context, IServiceBusBusFactoryConfigurator busFactoryConfigurator, Dictionary<string,string> subscriptions)
    {
        foreach (var (subscriptionKey,subscriptionValue) in subscriptions)
        {
            if (!string.IsNullOrEmpty(subscriptionValue))
            {
                busFactoryConfigurator.SubscriptionEndpoint(subscriptionValue,subscriptionKey, configurator =>
                {
                    switch (subscriptionKey)
                    {
                        case "CountryEvent":
                            configurator.ConfigureConsumer<CountryEventConsumer>(context);
                            break;
                        case "StateEvent":
                            configurator.ConfigureConsumer<StateEventConsumer>(context);
                            break;
                        case "CityEvent":
                            configurator.ConfigureConsumer<CityEventConsumer>(context);
                            break;
                    }
                                        
                    configurator.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10)));
                    configurator.UseMessageRetry(r => r.Immediate(3));
                                        
                    configurator.ConfigureDeadLetterQueueDeadLetterTransport();
                    configurator.ConfigureDeadLetterQueueErrorTransport();
                    configurator.PublishFaults = false;
                });
            }
        }
    }
    
    private static void RegisterSubscriptionOnRabbitMq(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator busFactoryConfigurator, Dictionary<string,string> subscriptions)
    {
        foreach (var (subscriptionKey,subscriptionValue) in subscriptions)
        {
            if (!string.IsNullOrEmpty(subscriptionValue))
            {
                busFactoryConfigurator.ReceiveEndpoint(subscriptionValue,configurator =>
                {
                    switch (subscriptionKey)
                    {
                        case "CountryEvent":
                            configurator.ConfigureConsumer<CountryEventConsumer>(context);
                            break;
                        case "StateEvent":
                            configurator.ConfigureConsumer<StateEventConsumer>(context);
                            break;
                        case "CityEvent":
                            configurator.ConfigureConsumer<CityEventConsumer>(context);
                            break;
                    }
                                        
                    configurator.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10)));
                    configurator.UseMessageRetry(r => r.Immediate(3));
                                        
                    configurator.ConfigureDeadLetterQueueDeadLetterTransport();
                    configurator.ConfigureDeadLetterQueueErrorTransport();
                    configurator.PublishFaults = false;
                });
            }
        }
    }
}
