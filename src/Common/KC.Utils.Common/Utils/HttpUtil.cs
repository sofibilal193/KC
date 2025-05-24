using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace KC.Utils.Common
{
    public static class HttpUtil
    {
        public static FormUrlEncodedContent ToFormData(this IDictionary<string, string> dict)
        {
            return new FormUrlEncodedContent(dict);
        }

        public static HttpContent? ToJsonHttpContent(this object content)
        {
            HttpContent? httpContent = null;

            if (content != null)
            {
                var ms = JsonUtil.SerializeJsonIntoStream(content);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }

        public static HttpContent? ToNameValuePairContent(this object content)
        {
            if (content != null)
            {
                var dict = content.ToDictionary();
                if (dict?.Count > 0)
                {
                    string nvpContent = string.Join("&",
                        dict.Select(kvp => string.Format("{0}={1}", kvp.Key, HttpUtility.UrlEncode(kvp.Value?.ToString())))
                    );
                    return new StringContent(nvpContent);
                }
            }

            return default;
        }

        public static string WithQueryStringParams(this string url, object content)
        {
            var dict = content.ToDictionary();
            if (dict?.Count > 0)
            {
                return string.Format("{0}?{1}", url, string.Join("&",
                    dict.Select(kvp => string.Format("{0}={1}", kvp.Key,
                    HttpUtility.UrlEncode(kvp.Value?.ToString())))));
            }
            return url;
        }
    }
}
