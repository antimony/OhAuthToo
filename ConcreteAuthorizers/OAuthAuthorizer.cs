﻿﻿﻿using OhAuthToo.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OhAuthToo.ConcreteAuthorizers
{
    public abstract class OAuthAuthorizer : IOAuthAuthorizer
    {
        private static readonly HttpClient httpClient = new HttpClient();
        
        public abstract string ClientName { get; }

        public string? ClientId { get; set; }
        protected virtual string? AuthorizeUri { get; set; }
        protected virtual string? TokenUri { get; set; }
        public string? RedirectUri { get; set; }
        public string? Scope { get; set; }
        public string? ClientSecret { get; set; }
        public virtual Dictionary<string, string>? Params { get; set; }
        public delegate void AuthorizationComplete(HttpResponseMessage response, string content);
        public event AuthorizationComplete? EndAuthorization;

        public string CodeRequestUri
        {
            get
            {
                if (string.IsNullOrEmpty(AuthorizeUri) || string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(RedirectUri))
                {
                    throw new InvalidOperationException("AuthorizeUri, ClientId, and RedirectUri must be set");
                }
                
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
            if (string.IsNullOrEmpty(TokenUri) || string.IsNullOrEmpty(ClientId) || 
                string.IsNullOrEmpty(ClientSecret) || string.IsNullOrEmpty(RedirectUri))
            {
                throw new InvalidOperationException("TokenUri, ClientId, ClientSecret, and RedirectUri must be set");
            }
            
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
            return GetAuthorizationResponseAsync(code).GetAwaiter().GetResult();
        }

        public async Task<string> GetAuthorizationResponseAsync(string code)
        {
            var uri = TokenRequestUri(code);
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(uri);
                string content = await response.Content.ReadAsStringAsync();
                
                EndAuthorization?.Invoke(response, content);
                return content;
            }
            catch (HttpRequestException e)
            {
                // Log the exception or handle it as needed
                return $"{{\"error\": {{\"message\": \"{e.Message}\"}}}}";
            }
        }
    }
}
