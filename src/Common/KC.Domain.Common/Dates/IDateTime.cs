using System;

namespace KC.Domain.Common
{
    public interface IDateTime
    {
        DateTime Now { get; }

        DateOnly DateNow { get; }

        int Year { get; }

        string ShortDateString { get; }
    }
}
