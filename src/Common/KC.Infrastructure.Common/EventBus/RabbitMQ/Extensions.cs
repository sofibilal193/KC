using Microsoft.Extensions.DependencyInjection;

namespace KC.Infrastructure.Common.EventBus.RabbitMQ
{
    public static class Extensions
    {
        /// <summary>
        /// Adds a service of the type <typeparamref name="TMessageProcessor"/> used to process messages of
        /// type <typeparamref name="TMessage"/> using Rabbit MQ to the specified <see cref="IServiceCollection">IServiceCollection</see>.
        /// </summary>
        /// <typeparam name="TMessageProcessor">Message processor type</typeparam>
        /// <typeparam name="TMessage">Message type</typeparam>
        /// <param name="services">Services collection to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddRabbitMQQueueProcessor<TMessageProcessor, TMessage>(this IServiceCollection services)
            where TMessageProcessor : class, IQueueProcessor<TMessage>
        {
            services.AddHostedService<RabbitMQQueueWorker<TMessage>>();
            services.AddSingleton<IQueueProcessor<TMessage>, TMessageProcessor>();
            return services;
        }

        /// <summary>
        /// Adds a service for sending messages to a Rabbit MQ queue.
        /// </summary>
        /// <param name="services">Services collection to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddRabbitMQQueueService(this IServiceCollection services)
        {
            return services.AddSingleton<IQueueService, RabbitMQQueueService>();
        }
    }
}
