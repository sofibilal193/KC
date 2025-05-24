namespace KC.Infrastructure.Common.RabbitMQ
{
    public record RabbitMQOptions
    {
        public string HostName { get; init; } = "";

        public string UserName { get; init; } = "";

        public string Password { get; init; } = "";

        public string BrokerName { get; init; } = "";

        public string QueueName { get; init; } = "";

        public int RetryCount { get; init; } = 5;

        public bool DispatchConsumersAsync { get; init; } = true;

        public RabbitMQOptions() { }

        public RabbitMQOptions(RabbitMQOptions options)
        {
            HostName = options.HostName;
            UserName = options.UserName;
            Password = options.Password;
            RetryCount = options.RetryCount;
            QueueName = options.QueueName;
            BrokerName = options.BrokerName;
            DispatchConsumersAsync = options.DispatchConsumersAsync;
        }
    }
}
