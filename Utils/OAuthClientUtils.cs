﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace OhAuthToo.Utils
{
    public static class OAuthClientUtils
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static JsonNode? JsonToDynamic(string response)
        {
            return JsonNode.Parse(response);
        }

        public static Dictionary<string, object>? JsonToDictionary(string response)
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(response);
        }

        public static string MakeRequest(string url)
        {
            return MakeRequestAsync(url).GetAwaiter().GetResult();
        }

        public static async Task<string> MakeRequestAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                // Log the exception or handle it as needed
                return $"{{\"error\": {{\"message\": \"{e.Message}\"}}}}";
            }
        }

        public static string RequestUrl(string baseurl, string requesturl, string token)
        {
             return baseurl + (requesturl.Contains("?")
                ?(requesturl + "&access_token=" + token)
                :(requesturl + "?access_token=" + token));
        }

        public static bool IsErrorResponse(string response)
        {
            var dict = JsonToDictionary(response);
            return dict != null && dict.ContainsKey("error");
        }
    }
}
