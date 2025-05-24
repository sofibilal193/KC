using System;
using System.Collections.Generic;
using System.Linq;
using KC.Domain.Common;
using KC.Domain.Common.ValueObjects;

namespace KC.Utils.Common
{
    public static class TagUtil
    {
        public static T? GetValue<T>(this List<Tag>? tags, string name)
        {
            var tag = tags?.Where(t => t.Name == name)?.OrderByDescending(t => t.CreateDateTimeUtc)?.FirstOrDefault();
            if (tag is not null)
            {
                return tag.Value.ConvertTo<T>();
            }
            return default;
        }

        public static T? GetValue<T>(this IList<TagDto> tags, string name)
        {
            var tag = tags.FirstOrDefault(t => t.Name == name);
            if (tag is not null)
            {
                return tag.Value.ConvertTo<T>();
            }
            return default;
        }

        public static string? GetValue(this IReadOnlyCollection<Tag>? tags, string name)
        {
            return tags?.Where(t => t.Name == name)?.OrderByDescending(t => t.CreateDateTimeUtc)?.FirstOrDefault()?.Value;
        }

        public static T? GetValue<T>(this IReadOnlyCollection<Tag>? tags, string name)
        {
            var tag = tags?.Where(t => t.Name == name)?.OrderByDescending(t => t.CreateDateTimeUtc)?.FirstOrDefault();
            if (tag is not null)
            {
                return tag.Value.ConvertTo<T>();
            }
            return default;
        }

        public static IList<T?>? GetValues<T>(this List<Tag>? tags, string name)
        {
            return tags?.Where(t => t.Name == name)?
                .OrderByDescending(t => t.CreateDateTimeUtc)?
                .Select(t => t.Value.ConvertTo<T>())?.ToList();
        }

        public static IList<string?>? GetValues(this IReadOnlyCollection<Tag>? tags, string name)
        {
            return tags?.Where(t => t.Name == name)?
                .OrderByDescending(t => t.CreateDateTimeUtc)?
                .Select(t => t.Value)?.ToList();
        }

        public static DateTime? GetDateTime(this List<Tag>? tags, string name)
        {
            var tag = tags?.Where(t => t.Name == name)?.OrderByDescending(t => t.CreateDateTimeUtc)?.FirstOrDefault();
            if (tag is not null)
            {
                return tag.CreateDateTimeUtc;
            }
            return default;
        }

        public static bool Exists(this IReadOnlyCollection<Tag>? tags, string name)
        {
            return !string.IsNullOrEmpty(tags?.FirstOrDefault(t => t.Name == name)?.Value);
        }
    }
}
