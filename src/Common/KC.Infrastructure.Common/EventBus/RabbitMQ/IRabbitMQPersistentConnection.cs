using System;
using RabbitMQ.Client;

namespace KC.Infrastructure.Common.RabbitMQ
{
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool IsAsyncConsumers { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
