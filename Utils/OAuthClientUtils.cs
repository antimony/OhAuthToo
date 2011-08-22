using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace OhAuthToo.Utils
{

    public static class OAuthClientUtils
    {
        public static dynamic JsonToDynamic(string response)
        {
            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });
            var dynamicResponse = jss.Deserialize(response, typeof(object)) as dynamic;
            return dynamicResponse;
        }

        public static Dictionary<string, object> JsonToDictionary(string response)
        {
            var jss = new JavaScriptSerializer();
            var responseDict = jss.Deserialize<Dictionary<string, object>>(response);
            return responseDict;
        }

        public static string MakeRequest(string url)
        {
            WebRequest myWebRequest = WebRequest.Create(url);
            WebResponse myWebResponse;
            try
            {
                myWebResponse = myWebRequest.GetResponse();
            }
            catch (WebException e)
            {
                myWebResponse = e.Response;
            }
            Stream receiveStream = myWebResponse.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(receiveStream, encode);
            string response = readStream.ReadToEnd();
            readStream.Close();
            myWebResponse.Close();
            return response;
        }

        public static string RequestUrl(string baseurl, string requesturl, string token)
        {
             return baseurl + (requesturl.Contains("?")
                ?(requesturl + "&access_token=" + token)
                :(requesturl + "?access_token=" + token));
        }

        public static bool IsErrorResponse(string response)
        {
            return (JsonToDictionary(response).ContainsKey("error"));
        }
    }
}
