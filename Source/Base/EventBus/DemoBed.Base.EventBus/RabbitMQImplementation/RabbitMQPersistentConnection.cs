using DemoBed.Base.EventBus.Abtraction;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Net.Sockets;

namespace DemoBed.Base.EventBus.RabbitMQImplementation
{
    /// <summary>
    /// Default RabbitMQ connection helper. Based on eShopOnContainers implementation.
    /// </summary>
    public class RabbitMQPersistentConnection : IPersistentConnection
    {
        private readonly IConnectionFactory connectionFactory;
        private readonly TimeSpan timeoutBeforeReconnecting;

        private IConnection connection;
        private bool disposed;

        private readonly object locker = new object();

        private readonly ILogger<RabbitMQPersistentConnection> logger;

        private bool connectionFailed = false;

        public RabbitMQPersistentConnection(
            IConnectionFactory connectionFactory,
            ILogger<RabbitMQPersistentConnection> logger,
            int timeoutBeforeReconnecting = 15)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this.logger = logger;
            this.timeoutBeforeReconnecting = TimeSpan.FromSeconds(timeoutBeforeReconnecting);
        }

        public event EventHandler OnReconnectedAfterConnectionFailure;

        public bool IsConnected
        {
            get
            {
                return connection != null && connection.IsOpen && !disposed;
            }
        }

        public bool TryConnect()
        {
            logger.LogInformation("Trying to connect to RabbitMQ...");

            lock (locker)
            {
                // Creates a policy to retry connecting to message broker until it succeds.
                var policy = Policy
                    .Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetryForever((duration) => timeoutBeforeReconnecting, (ex, time) =>
                    {
                        logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut} seconds ({ExceptionMessage}). Waiting to try again...", $"{(int)time.TotalSeconds}", ex.Message);
                    });

                policy.Execute(() =>
                {
                    connection = connectionFactory.CreateConnection();
                });

                if (!IsConnected)
                {
                    logger.LogCritical("ERROR: could not connect to RabbitMQ.");
                    connectionFailed = true;
                    return false;
                }

                // These event handlers hadle situations where the connection is lost by any reason. They try to reconnect the client.
                connection.ConnectionShutdown += OnConnectionShutdown;
                connection.CallbackException += OnCallbackException;
                connection.ConnectionBlocked += OnConnectionBlocked;
                connection.ConnectionUnblocked += OnConnectionUnblocked;

                logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.Endpoint.HostName);

                // If the connection has failed previously because of a RabbitMQ shutdown or something similar, we need to guarantee that the exchange and queues exist again.
                // It's also necessary to rebind all application event handlers. We use this event handler below to do this.
                if (connectionFailed)
                {
                    OnReconnectedAfterConnectionFailure?.Invoke(this, null);
                    connectionFailed = false;
                }

                return true;
            }
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action.");
            }

            return connection.CreateModel();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            try
            {
                connection.Dispose();
            }
            catch (IOException ex)
            {
                logger.LogCritical(ex.ToString());
            }
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs args)
        {
            connectionFailed = true;

            logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            TryConnectIfNotDisposed();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs args)
        {
            connectionFailed = true;

            logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            TryConnectIfNotDisposed();
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs args)
        {
            connectionFailed = true;

            logger.LogWarning("A RabbitMQ connection is blocked. Trying to re-connect...");
            TryConnectIfNotDisposed();
        }

        private void OnConnectionUnblocked(object sender, EventArgs args)
        {
            connectionFailed = true;

            logger.LogWarning("A RabbitMQ connection is unblocked. Trying to re-connect...");
            TryConnectIfNotDisposed();
        }

        private void TryConnectIfNotDisposed()
        {
            if (disposed)
            {
                logger.LogInformation("RabbitMQ client is disposed. No action will be taken.");
                return;
            }

            TryConnect();
        }
    }
}
