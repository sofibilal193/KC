using System;
using System.Collections.Generic;

namespace KC.Infrastructure.Common.ServiceBus
{
    public record ServiceBusOptions
    {
        public string Namespace { get; init; } = "";

        public string SubscriptionClientName { get; init; } = "";

        public int MaxConcurrentCalls { get; init; } = 10;

        public bool AutoCompleteMessages { get; init; }

        public Dictionary<string, ServiceBusQueueSettings> Queues { get; init; }

        public ServiceBusOptions()
        {
            Queues = new();
        }

        public ServiceBusOptions(ServiceBusOptions options)
        {
            Namespace = options.Namespace;
            SubscriptionClientName = options.SubscriptionClientName;
            MaxConcurrentCalls = options.MaxConcurrentCalls;
            AutoCompleteMessages = options.AutoCompleteMessages;
            Queues = new();
        }
    }

    public record ServiceBusTopicOption
    {
        public string Name { get; init; } = "";

        public bool RequiresDuplicateDetection { get; init; }

        public bool EnableBatchedOperations { get; init; } = true;

        public bool SupportOrdering { get; init; }

        public bool EnablePartitioning { get; init; }

        public long MaxSizeInMegabytes { get; init; } = 1024;

        public TimeSpan DefaultMessageTimeToLive { get; init; } = TimeSpan.MaxValue;

        public TimeSpan AutoDeleteOnIdle { get; init; } = TimeSpan.MaxValue;

        public TimeSpan DuplicateDetectionHistoryTimeWindow { get; init; } = TimeSpan.FromMinutes(1);
    }

    public record ServiceBusQueueSettings
    {
        public string QueueName { get; init; } = "";

        public int MaxConcurrentCalls { get; init; } = 10;

        public List<int> RetryDelaysSeconds { get; init; } = new();

        public int MaxRetryCount { get; init; }

        public bool IsDisabled { get; init; }

        public TimeSpan GetRetryDelay(int retryCount)
        {
            if (RetryDelaysSeconds.Count > 0)
            {
                var index = Math.Min(RetryDelaysSeconds.Count - 1, retryCount);
                return TimeSpan.FromSeconds(RetryDelaysSeconds[index]);
            }
            else
            {
                return TimeSpan.FromSeconds(60);
            }
        }
    }
}
