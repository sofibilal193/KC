using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace KC.Infrastructure.Common.RabbitMQ
{
    public class DefaultRabbitMQPersistentConnection
        : IRabbitMQPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        private readonly RabbitMQOptions _options;
        IConnection? _connection;
        bool _disposed;

        readonly object _syncRoot = new();

        public DefaultRabbitMQPersistentConnection(IConnectionFactory connectionFactory,
            ILogger<DefaultRabbitMQPersistentConnection> logger, RabbitMQOptions options)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options;
        }

        public bool IsAsyncConsumers
        {
            get
            {
                return (_connectionFactory as ConnectionFactory)?.DispatchConsumersAsync == true;
            }
        }

        public bool IsConnected
        {
            get
            {
                return _connection?.IsOpen == true && !_disposed;
            }
        }

        public IModel CreateModel()
        {
            if (!IsConnected || _connection is null)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        #region IDisposable Methods

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                // dispose resources
                _disposed = true;
                if (_connection is not null)
                {
                    try
                    {
                        _connection.ConnectionShutdown -= OnConnectionShutdown!;
                        _connection.CallbackException -= OnCallbackException!;
                        _connection.ConnectionBlocked -= OnConnectionBlocked!;
                        _connection.Dispose();
                    }
                    catch (IOException ex)
                    {
                        _logger.LogError(ex, "An error has occurred while disposing the connection");
                    }
                }
            }
        }

        #endregion

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (_syncRoot)
            {
                var policy = RetryPolicy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_options?.RetryCount ?? 5, retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time)
                            => _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})",
                                $"{time.TotalSeconds:n1}", ex.Message)
                );

                policy.Execute(() =>
                {
                    _connection = _connectionFactory
                            .CreateConnection();
                });

                if (IsConnected && _connection is not null)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown!;
                    _connection.CallbackException += OnCallbackException!;
                    _connection.ConnectionBlocked += OnConnectionBlocked!;

                    _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.Endpoint.HostName);

                    return true;
                }
                else
                {
                    _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                    return false;
                }
            }
        }

        [ExcludeFromCodeCoverage]
        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            TryConnect();
        }

        [ExcludeFromCodeCoverage]
        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        [ExcludeFromCodeCoverage]
        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }
    }
}
