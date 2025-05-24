namespace KC.Domain.Common.Exceptions
{
#pragma warning disable S3925 // Update this implementation of 'ISerializable' to conform to the recommended serialization pattern.
#pragma warning disable RCS1194 // Implement exception constructors.
    /// <summary>
    /// Exception that will cause a service bus message to be requeued for processing.
    /// Note: This exception should not be thrown when processing batches of messages
    /// (multiple messages in session) if the messages need to be processed in order.
    /// </summary>
    public class RetryableException : Exception
    {
        public int? MaxRetryCount { get; private set; }

        public List<int>? RetryDelaysSeconds { get; private set; }

        /// <summary>
        /// Creates a new RetryableException with the specified message.
        /// </summary>
        /// <param name="message">Error message to log. Message will also be attached to
        /// service bus message when sent to dead letter queue.</param>
        /// <param name="maxRetryCount">Maximum number of times to retry if thrown.</param>
        /// <param name="retryDelaySeconds">Retry delay in seconds
        /// (can be different for each retry by specifying multiple).</param>
        public RetryableException(string message, int? maxRetryCount = null,
            params int[]? retryDelaySeconds) : base(message)
        {
            MaxRetryCount = maxRetryCount;
            RetryDelaysSeconds = retryDelaySeconds?.ToList();
        }

        /// <summary>
        /// Creates a new RetryableException with the specified inner exception.
        /// </summary>
        /// <param name="innerException">Exception to be logged. Exception error message will
        /// also be attached to service bus message when sent to dead letter queue</param>
        /// <param name="maxRetryCount">Maximum number of times to retry if thrown.</param>
        /// <param name="retryDelaySeconds">Retry delay in seconds
        /// (can be different for each retry by specifying multiple).</param>
        public RetryableException(Exception innerException, int? maxRetryCount = null,
            params int[]? retryDelaySeconds) : base(null, innerException)
        {
            MaxRetryCount = maxRetryCount;
            RetryDelaysSeconds = retryDelaySeconds?.ToList();
        }

        public TimeSpan? GetRetryDelay(int retryCount)
        {
            if (RetryDelaysSeconds?.Count > 0)
            {
                var index = Math.Min(RetryDelaysSeconds.Count - 1, retryCount);
                return TimeSpan.FromSeconds(RetryDelaysSeconds[index]);
            }
            return null;
        }
    }
#pragma warning restore RCS1194 // Implement exception constructors.
#pragma warning restore S3925 // Update this implementation of 'ISerializable' to conform to the recommended serialization pattern.
}
