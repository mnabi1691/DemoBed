using DemoBed.Base.EventBus.Abtraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreEventBus.Infra.EventBus.Events;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DemoBed.Base.EventBus.RabbitMQImplementation
{
    public class RabbitMQEventBus : IEventBus
    {
        private readonly string exchangeName;
        private readonly string queueName;
        private readonly int publishRetryCount = 5;
        private readonly TimeSpan subscribeRetryTime = TimeSpan.FromSeconds(5);

        private readonly IPersistentConnection persistentConnection;
        private readonly IEventBusSubscriptionManager subscriptionsManager;
        private readonly IServiceProvider serviceProvider;
        private readonly IServiceScopeFactory scopeFactory;

        private readonly ILogger<RabbitMQEventBus> logger;

        private IModel? consumerChannel;

        public RabbitMQEventBus(
            IPersistentConnection persistentConnection,
            IEventBusSubscriptionManager subscriptionsManager,
            IServiceProvider serviceProvider,
            IServiceScopeFactory scopeFactory,
            ILogger<RabbitMQEventBus> logger,
            string brokerName,
            string queueName)
        {
            this.persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            this.subscriptionsManager = subscriptionsManager ?? throw new ArgumentNullException(nameof(subscriptionsManager));
            this.serviceProvider = serviceProvider;
            this.scopeFactory = scopeFactory;
            this.logger = logger;
            this.exchangeName = brokerName ?? throw new ArgumentNullException(nameof(brokerName));
            this.queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));

            ConfigureMessageBroker();
        }

        public void Publish<TEvent>(TEvent @event)
            where TEvent : Event
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var policy = Policy
                .Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(publishRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan) =>
                {
                    logger.LogWarning(exception, "Could not publish event #{EventId} after {Timeout} seconds: {ExceptionMessage}.", @event.Id, $"{timeSpan.TotalSeconds:n1}", exception.Message);
                });

            var eventName = @event.GetType().Name;

            logger.LogTrace("Creating RabbitMQ channel to publish event #{EventId} ({EventName})...", @event.Id, eventName);

            using (var channel = persistentConnection.CreateModel())
            {
                logger.LogTrace("Declaring RabbitMQ exchange to publish event #{EventId}...", @event.Id);

                channel.ExchangeDeclare(exchange: exchangeName, type: "direct");

                var message = JsonSerializer.Serialize(@event);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = (byte)DeliveryMode.Persistent;

                    logger.LogTrace("Publishing event to RabbitMQ with ID #{EventId}...", @event.Id);

                    channel.BasicPublish(
                        exchange: exchangeName,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);

                    logger.LogTrace("Published event with ID #{EventId}.", @event.Id);
                });
            }
        }

        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = subscriptionsManager.GetEventIdentifier<TEvent>();
            var eventHandlerName = typeof(TEventHandler).Name;

            AddQueueBindForEventSubscription(eventName);

            logger.LogInformation("Subscribing to event {EventName} with {EventHandler}...", eventName, eventHandlerName);

            subscriptionsManager.AddSubscription<TEvent, TEventHandler>();
            StartBasicConsume();

            logger.LogInformation("Subscribed to event {EventName} with {EvenHandler}.", eventName, eventHandlerName);
        }

        public void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = subscriptionsManager.GetEventIdentifier<TEvent>();

            logger.LogInformation("Unsubscribing from event {EventName}...", eventName);

            subscriptionsManager.RemoveSubscription<TEvent, TEventHandler>();

            logger.LogInformation("Unsubscribed from event {EventName}.", eventName);
        }

        private void ConfigureMessageBroker()
        {
            consumerChannel = CreateConsumerChannel();
            subscriptionsManager.OnEventRemoved += SubscriptionManager_OnEventRemoved;
            persistentConnection.OnReconnectedAfterConnectionFailure += PersistentConnection_OnReconnectedAfterConnectionFailure;
        }

        private IModel CreateConsumerChannel()
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            logger.LogTrace("Creating RabbitMQ consumer channel...");

            var channel = persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: exchangeName, type: "direct");
            channel.QueueDeclare
            (
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            channel.CallbackException += (sender, ea) =>
            {
                logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel...");
                DoCreateConsumerChannel();
            };

            logger.LogTrace("Created RabbitMQ consumer channel.");


            return channel;
        }

        private void StartBasicConsume()
        {
            logger.LogTrace("Starting RabbitMQ basic consume...");

            if (consumerChannel == null)
            {
                logger.LogError("Could not start basic consume because consumer channel is null.");
                return;
            }

            var consumer = new AsyncEventingBasicConsumer(consumerChannel);
            consumer.Received += Consumer_Received;

            consumerChannel.BasicConsume
            (
                queue: queueName,
                autoAck: false,
                consumer: consumer
            );

            logger.LogTrace("Started RabbitMQ basic consume.");
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            bool isAcknowledged = false;

            try
            {
                await ProcessEvent(eventName, message);

                consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                isAcknowledged = true;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error processing the following message: {Message}.", message);
            }
            finally
            {
                if (!isAcknowledged)
                {
                    await TryEnqueueMessageAgainAsync(eventArgs);
                }
            }
        }

        private async Task TryEnqueueMessageAgainAsync(BasicDeliverEventArgs eventArgs)
        {
            try
            {
                logger.LogWarning("Adding message to queue again with {Time} seconds delay...", $"{subscribeRetryTime.TotalSeconds:n1}");

                await Task.Delay(subscribeRetryTime);
                consumerChannel.BasicNack(eventArgs.DeliveryTag, false, true);

                logger.LogTrace("Message added to queue again.");
            }
            catch (Exception ex)
            {
                logger.LogError("Could not enqueue message again: {Error}.", ex.Message);
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            logger.LogTrace("Processing RabbitMQ event: {EventName}...", eventName);

            if (!subscriptionsManager.HasSubscriptionsForEvent(eventName))
            {
                logger.LogTrace("There are no subscriptions for this event.");
                return;
            }

            var subscriptions = subscriptionsManager.GetHandlersForEvent(eventName);
            foreach (var subscription in subscriptions)
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                        
                    if (handler == null)
                    {
                        logger.LogWarning("There are no handlers for the following event: {EventName}", eventName);
                        continue;
                    }

                    var eventType = subscriptionsManager.GetEventTypeByName(eventName);

                    var @event = JsonSerializer.Deserialize(message, eventType);
                    var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType) ?? throw new ArgumentNullException();
                    await Task.Yield();
                    await (Task)eventHandlerType.GetMethod(nameof(IEventHandler<Event>.HandleAsync)).Invoke(handler, new object[] { @event });
                }
            }

            logger.LogTrace("Processed event {EventName}.", eventName);
        }

        private void SubscriptionManager_OnEventRemoved(object sender, string eventName)
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            using (var channel = persistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: queueName, exchange: exchangeName, routingKey: eventName);

                if (subscriptionsManager.IsEmpty)
                {
                    consumerChannel.Close();
                }
            }
        }

        private void AddQueueBindForEventSubscription(string eventName)
        {
            var containsKey = subscriptionsManager.HasSubscriptionsForEvent(eventName);
            if (containsKey)
            {
                return;
            }

            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            using (var channel = persistentConnection.CreateModel())
            {
                channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: eventName);
            }
        }

        private void PersistentConnection_OnReconnectedAfterConnectionFailure(object sender, EventArgs e)
        {
            DoCreateConsumerChannel();
            RecreateSubscriptions();
        }

        private void DoCreateConsumerChannel()
        {
            consumerChannel.Dispose();
            consumerChannel = CreateConsumerChannel();
            StartBasicConsume();
        }

        private void RecreateSubscriptions()
        {
            var subscriptions = subscriptionsManager.GetAllSubscriptions();
            subscriptionsManager.Clear();

            Type eventBusType = GetType();
            MethodInfo genericSubscribe;

            foreach (var entry in subscriptions)
            {
                foreach (var subscription in entry.Value)
                {
                    genericSubscribe = eventBusType.GetMethod("Subscribe").MakeGenericMethod(subscription.EventType, subscription.HandlerType);
                    genericSubscribe.Invoke(this, null);
                }
            }
        }
    }
}