using Microsoft.Extensions.DependencyInjection;

namespace KC.Infrastructure.Common.EventBus.ServiceBus
{
    public static class Extensions
    {
        /// <summary>
        /// Adds a service of the type <typeparamref name="TMessageProcessor"/> used to process messages of
        /// type <typeparamref name="TMessage"/> using Azure Service Bus to the specified <see cref="IServiceCollection">IServiceCollection</see>.
        /// </summary>
        /// <typeparam name="TMessageProcessor">Message processor type</typeparam>
        /// <typeparam name="TMessage">Message type</typeparam>
        /// <param name="services">Services collection to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddServiceBusQueueProcessor<TMessageProcessor, TMessage>(this IServiceCollection services)
            where TMessageProcessor : class, IQueueProcessor<TMessage>
        {
            services.AddHostedService<ServiceBusQueueWorker<TMessage>>();
            services.AddSingleton<IServiceBusAdministrativeClientFactory, ServiceBusAdministrativeClientFactory>();
            services.AddSingleton<IServiceBusClientFactory, ServiceBusClientFactory>();
            services.AddSingleton<IQueueProcessor<TMessage>, TMessageProcessor>();
            return services;
        }

        /// <summary>
        /// Adds a service for sending messages to a Azure Service Bus queue.
        /// </summary>
        /// <param name="services">Services collection to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddServiceBusQueueService(this IServiceCollection services)
        {
            services.AddSingleton<IServiceBusAdministrativeClientFactory, ServiceBusAdministrativeClientFactory>();
            services.AddSingleton<IServiceBusClientFactory, ServiceBusClientFactory>();
            return services.AddSingleton<IQueueService, ServiceBusQueueService>();
        }
    }
}
