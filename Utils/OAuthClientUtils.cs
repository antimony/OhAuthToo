using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OhAuthToo.Utils
{

    public static class OAuthClientUtils
    {
        public static string GenerateState()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static string BuildQueryString(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.Any())
                return string.Empty;

            return "?" + string.Join("&", parameters.Select(p => $"{UrlEncode(p.Key)}={UrlEncode(p.Value)}"));
        }

        public static Dictionary<string, string> ParseQueryString(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
                return new Dictionary<string, string>();

            var result = new Dictionary<string, string>();
            var pairs = queryString.TrimStart('?').Split('&');

            foreach (var pair in pairs)
            {
                var parts = pair.Split('=');
                if (parts.Length == 2)
                {
                    var key = UrlDecode(parts[0]);
                    var value = UrlDecode(parts[1]);
                    result[key] = value;
                }
            }

            return result;
        }

        public static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return Uri.EscapeDataString(value);
        }

        public static string UrlDecode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return Uri.UnescapeDataString(value);
        }

        public static JsonNode JsonToDynamic(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            return JsonNode.Parse(json);
        }

        public static Dictionary<string, string> JsonToDictionary(string json)
        {
            if (string.IsNullOrEmpty(json))
                return new Dictionary<string, string>();

            var jsonNode = JsonNode.Parse(json);
            if (jsonNode == null)
                return new Dictionary<string, string>();

            var result = new Dictionary<string, string>();
            foreach (var property in jsonNode.AsObject())
            {
                result[property.Key] = property.Value?.ToString();
            }

            return result;
        }

        public static string RequestUrl(string baseUrl, string requestUrl, string token)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (string.IsNullOrEmpty(requestUrl))
                throw new ArgumentNullException(nameof(requestUrl));

            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));

            var separator = requestUrl.Contains("?") ? "&" : "?";
            return $"{baseUrl}{requestUrl}{separator}access_token={UrlEncode(token)}";
        }

        public static bool IsErrorResponse(string json)
        {
            if (string.IsNullOrEmpty(json))
                return false;

            try
            {
                var jsonNode = JsonNode.Parse(json);
                return jsonNode?["error"] != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
