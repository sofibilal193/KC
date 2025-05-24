using System.Threading.Tasks;

namespace KC.Infrastructure.Common.EventBus
{
    /// <summary>
    /// Interface for implementing a queue message processor that process messages of type <typeparamref name="TMessage"/>.
    /// </summary>
    /// <typeparam name="TMessage">Type of message that is processed.</typeparam>
    public interface IQueueProcessor<in TMessage>
    {
        /// <summary>
        /// Processes a message from a service bus queue.
        /// </summary>
        /// <param name="message">Message to process.</param>
        /// <returns>An awaitable task.</returns>
        Task ProcessMessageAsync(TMessage message, bool isFinalTry);
    }
}
