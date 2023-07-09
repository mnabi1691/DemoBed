using DemoBed.Base.EventBus.Abtraction;
using DemoBed.Base.EventBus.InMemorySubsriptionImplementation;
using DemoBed.Base.EventBus.RabbitMQImplementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace DemoBed.Base.EventBus.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMQEventBus(
            this IServiceCollection services,
            string connectionUrl, 
            string brokerName, 
            string queueName, 
            int timeoutBeforeReconnecting = 15)
        {
            services.AddSingleton<IEventBusSubscriptionManager, InMemoryEventBusSubscriptionManager>();
            services.AddSingleton<IPersistentConnection, RabbitMQPersistentConnection>(provider =>
            {
                var connectionFactory = new ConnectionFactory
                {
                    Uri = new Uri(connectionUrl),
                    DispatchConsumersAsync = true,
                };

                var logger = provider.GetRequiredService<ILogger<RabbitMQPersistentConnection>>();
                return new RabbitMQPersistentConnection(connectionFactory, logger, timeoutBeforeReconnecting);
            });

            services.AddSingleton<IEventBus, RabbitMQEventBus>(provider =>
            {
                return new RabbitMQEventBus(
                    provider.GetRequiredService<IPersistentConnection>() ?? throw new ArgumentNullException(),
                    provider.GetRequiredService<IEventBusSubscriptionManager>() ?? throw new ArgumentNullException(), 
                    provider ?? throw new ArgumentNullException(),
                    provider.GetRequiredService<ILogger<RabbitMQEventBus>>() ?? throw new ArgumentNullException(), 
                    brokerName, 
                    queueName);
            });

            return services;
        }
    }
}
