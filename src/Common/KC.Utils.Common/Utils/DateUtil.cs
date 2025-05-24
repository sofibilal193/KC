using System;
using System.Globalization;
using KC.Domain.Common;
using KC.Domain.Common.Constants;

namespace KC.Utils.Common
{
    public static class DateUtil
    {
        public static DateTime? ToDateTime(this string? s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (DateTime.TryParse(s, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out var dateResult))
                {
                    return dateResult;
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return default;
            }
        }

        public static DateTime? ToDateTime(this DateOnly? dt)
        {
            if (dt.HasValue)
                return dt.Value.ToDateTime(TimeOnly.MinValue);
            else
                return default;
        }

        public static DateTime ToDateTime(this DateOnly dt)
        {
            return dt.ToDateTime(TimeOnly.MinValue);
        }

        public static DateTime? UtcToTimeZoneTime(DateTime? dtUtc, string timeZoneId)
        {
            if (dtUtc.HasValue)
            {
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(dtUtc.Value, tz);
            }
            return default;
        }

        public static DateTime UtcToTimeZoneTime2(DateTime dtUtc, string timeZoneId)
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(dtUtc, tz);
        }

        public static string UtcToEstString(this DateTime dtUtc)
        {
            return UtcToTimeZoneTime(dtUtc, TimeZones.USEasternTime)?.ToString("M/d/yyyy h:mm tt") + " EST";
        }

        public static int YearsDiff(DateTime startDate, DateTime endDate)
        {
            int age = endDate.Year - startDate.Year;
            if (startDate > endDate.AddYears(-age)) age--;
            return age;
        }

        public static int? YearsDiff(DateOnly? startDate, DateOnly? endDate)
        {
            if (startDate is not null && endDate is not null)
            {
                int age = endDate.Value.Year - startDate.Value.Year;
                if (startDate.Value > endDate.Value.AddYears(-age)) age--;
                return age;
            }
            return default;
        }

        public static int MonthsDiff(DateTime startDate, DateTime endDate)
        {
            var months = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
            if (startDate.Day > endDate.Day) months--;
            return months;
        }

        public static double DaysDiff(DateTime startDate, DateTime endDate)
        {
            return (endDate.Date - startDate.Date).TotalDays;
        }

        public static double HoursDiff(DateTime startDate, DateTime endDate)
        {
            return (endDate - startDate).TotalHours;
        }

        public static double MinutesDiff(DateTime startDate, DateTime endDate)
        {
            return (endDate - startDate).TotalMinutes;
        }

        public static double SecondsDiff(DateTime startDate, DateTime endDate)
        {
            return (endDate - startDate).TotalSeconds;
        }

        public static DateOnly ToDateOnly(this DateTime dt)
        {
            return DateOnly.FromDateTime(dt);
        }

        public static DateOnly? ToDateOnly(this DateTime? dt)
        {
            if (dt.HasValue)
                return DateOnly.FromDateTime(dt.Value);
            else
                return default;
        }

        public static FilterDateDto GetFilterDates(CalendarType type, DateOnly dateToCheck, DateOnly? customEndDate)
        {
            DateOnly dateRangeBegin = dateToCheck;
            DateOnly dateRangeEnd = default;
            DateOnly lastDateRangeBegin = dateToCheck;
            DateOnly lastDateRangeEnd = default;

            switch (type)
            {
                case CalendarType.Weekly:
                    dateRangeBegin = dateToCheck.AddDays(-(int)dateToCheck.DayOfWeek);
                    dateRangeEnd = dateToCheck.AddDays(6 - (int)dateToCheck.DayOfWeek);
                    lastDateRangeBegin = dateRangeBegin.AddDays(-7);
                    lastDateRangeEnd = dateRangeBegin.AddDays(-1);
                    break;

                case CalendarType.Monthly:
                    var lastDayOfMonth = DateTime.DaysInMonth(dateToCheck.Year, dateToCheck.Month);
                    dateRangeBegin = dateToCheck.AddDays(((-1) * dateToCheck.Day) + 1);
                    dateRangeEnd = dateRangeBegin.AddDays(lastDayOfMonth - 1);
                    lastDateRangeBegin = dateRangeBegin.AddMonths(-1);
                    lastDateRangeEnd = dateRangeBegin.AddDays(-1);
                    break;

                case CalendarType.Annually:
                    dateRangeBegin = new DateOnly(dateToCheck.Year, 1, 1);
                    dateRangeEnd = new DateOnly(dateToCheck.Year, 12, 31);
                    lastDateRangeBegin = dateRangeBegin.AddYears(-1);
                    lastDateRangeEnd = dateRangeBegin.AddDays(-1);
                    break;

                case CalendarType.Custom:
                    dateRangeBegin = dateToCheck;
                    dateRangeEnd = customEndDate ?? default;
                    lastDateRangeBegin = dateRangeBegin.AddYears(-1);
                    lastDateRangeEnd = dateRangeEnd.AddYears(-1);
                    break;
            }

            var filterDateDto = new FilterDateDto
            {
                DateRangeBegin = dateRangeBegin,
                DateRangeEnd = dateRangeEnd,
                LastDateRangeBegin = lastDateRangeBegin,
                LastDateRangeEnd = lastDateRangeEnd
            };

            return filterDateDto;
        }

        public static int? MonthsRemaining(this DateOnly? dt)
        {
            if (!dt.HasValue) return default;

            var months = 0;
            var start = DateTime.UtcNow.Date.ToDateOnly();
            var end = dt.Value;
            if (end > start)
            {
                months = ((end.Year - start.Year) * 12) + (end.Month - start.Month);
                if (start.Day > end.Day)
                {
                    months--;
                }
            }
            return months < 0 ? 0 : months;
        }
    }
}
