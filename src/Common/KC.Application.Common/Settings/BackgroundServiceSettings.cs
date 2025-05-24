using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace KC.Application.Common.Settings
{
    [SuppressMessage("Maintainability", "S4004", Justification = "Set is needed for instantiation and binding from appsettings.json.")]
    public class BackgroundServiceSettings
    {
        public Dictionary<string, BackgroundServiceConfig> Services { get; set; } = new();
    }

    [SuppressMessage("Maintainability", "S6566", Justification = "DateTimeOffset is not needed as we are doing everything in UTC time.")]
    [SuppressMessage("Maintainability", "S6354", Justification = "Testable provider is not needed as we are doing everything in UTC time.")]
    public class BackgroundServiceConfig
    {
        public bool IsEnabled { get; init; }

        public int? StartDelaySeconds { get; init; }

        public int? IntervalPeriodSeconds { get; init; }

        public int? IntervalPeriodMilliseconds { get; init; }

        public TimeSpan? DailyStartTimeUtc { get; init; }

        public string? WorkerName { get; init; }

        public TimeSpan StartDelay
        {
            get
            {
                if (DailyStartTimeUtc.HasValue)
                {
                    var start = DateTime.UtcNow.Date.Add(DailyStartTimeUtc.Value);
                    // if start time has already elapsed, set to same start time tomorrow
                    if (start < DateTime.UtcNow)
                    {
                        start = DateTime.UtcNow.Date.AddDays(1).Add(DailyStartTimeUtc.Value);
                    }
                    return start.Subtract(DateTime.UtcNow);
                }
                return TimeSpan.FromSeconds(StartDelaySeconds ?? 0);
            }
        }

        public TimeSpan? IntervalPeriod
        {
            get
            {
                // if a daily start time is set, interval period should run at same time every day
                if (DailyStartTimeUtc.HasValue)
                {
                    return StartDelay;
                }
                else if (IntervalPeriodMilliseconds.HasValue)
                {
                    return TimeSpan.FromMilliseconds(IntervalPeriodMilliseconds.Value);
                }
                else if (IntervalPeriodSeconds.HasValue)
                {
                    return TimeSpan.FromSeconds(IntervalPeriodSeconds.Value);
                }
                else
                {
                    // returns default at the end anyways
                }
                return default;
            }
        }
    }
}
