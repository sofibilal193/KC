using System;

namespace KC.Domain.Common
{
    public class UtcDateTime : IDateTime
    {
        public DateTime Now => DateTime.UtcNow;

        public DateOnly DateNow => DateOnly.FromDateTime(Now);

        public int Year => Now.Year;

        public string ShortDateString => Now.ToShortDateString();
    }
}
