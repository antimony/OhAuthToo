using System.IO;
using OhAuthToo.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace OhAuthToo.ConcreteAuthorizers
{
    public abstract class OAuthAuthorizer:IOAuthAuthorizer
    {
        public abstract string ClientName { get; }

        public string ClientId { get; set; }
        protected virtual string AuthorizeUri { get; set; }
        protected virtual string TokenUri { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
        public string ClientSecret { get; set; }
        public virtual Dictionary<string,string> Params { get; set; } 
        public delegate void AuthorizationComplete(DownloadStringCompletedEventArgs args);
        public event AuthorizationComplete EndAuthorization;

        public string CodeRequestUri
        {
            get
            {
                var query = new StringBuilder();
                query.AppendFormat("?client_id={0}", ClientId);
                query.AppendFormat("&redirect_uri={0}", RedirectUri);
                if (!string.IsNullOrEmpty(Scope))
                {
                    query.AppendFormat("&scope={0}", Scope);
                }
                if (Params != null)
                {
                    foreach (var param in Params)
                    {
                        query.AppendFormat("&{0}={1}", param.Key, param.Value);
                    }
                }
                return AuthorizeUri + query;
            }
        }

        private Uri TokenRequestUri(string code)
        {
                var query = new StringBuilder();
                query.AppendFormat("?client_id={0}", ClientId);
                query.AppendFormat("&client_secret={0}", ClientSecret);
                query.AppendFormat("&redirect_uri={0}", RedirectUri);
                query.AppendFormat("&code={0}", code);
                if (Params != null)
                {
                    foreach (var param in Params)
                    {
                        query.AppendFormat("&{0}={1}", param.Key, param.Value);
                    }
                }
                return new Uri(TokenUri + query);
        }

        public string GetAuthorizationResponse(string code)
        {
            var uri = TokenRequestUri(code);
            var myWebRequest = WebRequest.Create(uri);
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
            Encoding encode = Encoding.GetEncoding("utf-8");
            var readStream = new StreamReader(receiveStream, encode);
            string response = readStream.ReadToEnd();
            readStream.Close();
            myWebResponse.Close();
            return response;
        }

        public void GetAuthorizationResponseAsync(string code)
        {
            var wc = new WebClient();
            var uri = TokenRequestUri(code);
            wc.DownloadStringCompleted += delegate(object o, DownloadStringCompletedEventArgs eventArgs)
                                              {
                                                  if (EndAuthorization!=null)
                                                  {
                                                      EndAuthorization(eventArgs);
                                                  }
                                              };
            wc.DownloadStringAsync(uri);
        }
    }
}
