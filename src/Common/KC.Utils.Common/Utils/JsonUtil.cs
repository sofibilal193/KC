using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace KC.Utils.Common
{
    public static class JsonUtil
    {
        public static Stream? GetJsonStream(this string json, string partitionKeyPath,
            string partitionKey, string? id = null, string? eTag = null)
        {
            if (!string.IsNullOrEmpty(json))
            {
                var rootNode = JsonNode.Parse(json);
                if (rootNode != null)
                {
                    rootNode["id"] = id ?? Guid.NewGuid().ToString();
                    rootNode[partitionKeyPath] = partitionKey;
                    if (!string.IsNullOrEmpty(eTag))
                        rootNode["_etag"] = eTag;
                    var stream = new MemoryStream();
                    using var writer = new Utf8JsonWriter(stream);
                    rootNode.WriteTo(writer);
                    writer.Flush();
                    return stream;
                }
            }
            return null;
        }

        public static MemoryStream SerializeJsonIntoStream(object value)
        {
            var stream = new MemoryStream();
            JsonSerializer.Serialize(stream, value, value.GetType(), new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            return stream;
        }

        public static T? TryParseTo<T>(this string s)
        {
            T? val = default;

            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    val = JsonSerializer.Deserialize<T>(s, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch
                {
                    // ignore, default value will be returned
                }
            }

            return val;
        }

        public static T? ParseJsonFile<T>(string path)
        {
            T? val = default;

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                val = JsonSerializer.Deserialize<T>(File.ReadAllText(path));
            }
            return val;
        }

        public static string Serialize<T>(this T o)
        {
            return JsonSerializer.Serialize<T>(o, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }

        public static Dictionary<string, object?>? ToDictionary<T>(this T o)
        {
            var json = o.Serialize();
            if (!string.IsNullOrEmpty(json))
            {
                return JsonSerializer.Deserialize<Dictionary<string, object?>>(json, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });
            }
            else
            {
                return default;
            }
        }
    }
}
