using RabbitMQ.Client;
using System;

namespace DemoBed.Base.EventBus.Abtraction
{
    public interface IPersistentConnection
    {
        event EventHandler OnReconnectedAfterConnectionFailure;
        bool IsConnected { get; }

        bool TryConnect();
        IModel CreateModel();
    }
}