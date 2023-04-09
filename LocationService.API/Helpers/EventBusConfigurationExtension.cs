 using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Reflection;
 using LocationService.Application.Interfaces;
 using LocationService.Infrastructure.Bus;
 using LocationService.Message.Definition.Cities.Events.v1;
 using LocationService.Message.Definition.Countries.Events.v1;
 using LocationService.Message.Definition.States.Events.v1;
 using MassTransit;
 using MediatR;
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
                            RegisterSubscriptionDynamically(context, eventBusOptions.Subscriptions, cfg);
                            cfg.ConfigureEndpoints(context);
                        });
                        break;
                    case "RabbitMQ":
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host(new Uri(eventBusOptions.ConnectionString));

                            SetEventBusMessages(cfg);
                            RegisterSubscriptionDynamically(context, eventBusOptions.Subscriptions, cfg);
                            cfg.ConfigureEndpoints(context);
                        });
                        break;
                }
            });
        }
        
        services.AddSingleton(eventBusOptions);
        services.AddScoped<IEventBus, EventBus>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(EventBusBehaviour<,>));
        return services;
    }

    private static void SetEventBusMessages<T>(IBusFactoryConfigurator<T> busFactoryConfigurator) where T : IReceiveEndpointConfigurator
    {
        busFactoryConfigurator.Message<CountryEvent>(e => e.SetEntityName(nameof(CountryEvent)));
        busFactoryConfigurator.Message<StateEvent>(e => e.SetEntityName(nameof(StateEvent)));
        busFactoryConfigurator.Message<CityEvent>(e => e.SetEntityName(nameof(CityEvent)));
    }

    private static void RegisterSubscriptionDynamically<T>(IRegistrationContext context, Dictionary<string, string> subscriptions, IBusFactoryConfigurator<T> busFactoryConfigurator) where T : IReceiveEndpointConfigurator
    {
        foreach (var (subscriptionKey,subscriptionValue) in subscriptions)
        {
            if (!string.IsNullOrWhiteSpace(subscriptionValue))
            {
                switch (busFactoryConfigurator)
                {
                    case IServiceBusBusFactoryConfigurator azureBusConfigurator:
                        azureBusConfigurator.SubscriptionEndpoint(subscriptionValue,subscriptionKey, configurator =>
                        {
                            AddConsumersFromConfiguration(context, configurator, subscriptionKey);
                        });
                        break;
                    case IRabbitMqBusFactoryConfigurator rabbitBusConfigurator:
                        rabbitBusConfigurator.ReceiveEndpoint(subscriptionValue, configurator =>
                        {
                            AddConsumersFromConfiguration(context, configurator, subscriptionKey);
                        });
                        break;
                }
            }
        }
    }

    private static void AddConsumersFromConfiguration(IRegistrationContext context, IReceiveEndpointConfigurator configurator, string subscriptionKey)
    {
        var interfaceType = typeof(IConsumer);
        var consumerTypes =
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ToList();
        
        foreach (var consumerType in consumerTypes.Where(consumerType => consumerType.Name == $"{subscriptionKey}Consumer"))
        {
            configurator.ConfigureConsumer(context, consumerType);
        }
                                        
        configurator.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10)));
        configurator.UseMessageRetry(r => r.Immediate(3));
                                        
        configurator.ConfigureDeadLetterQueueDeadLetterTransport();
        configurator.ConfigureDeadLetterQueueErrorTransport();
        configurator.PublishFaults = false;
    }
}
