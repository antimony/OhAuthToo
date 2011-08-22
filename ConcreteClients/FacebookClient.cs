using OhAuthToo.Interfaces;
using OhAuthToo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Web;

namespace OhAuthToo.ConcreteClients
{
    public class FacebookClient : IOAuthClient
    {
        private string userId;
        private string me;
        public string BaseRequestUrl
        {
            get { return "https://graph.facebook.com"; }
        }

        public string ClientName
        {
            get { return "Facebook"; }
        }

        public string Response
        {
            set { GetDataFromResponse(value); }
        }

        public string Token { get; set; }
        public string UserId { get
        {
            if (string.IsNullOrEmpty(userId))
            {
                var dict = OAuthClientUtils.JsonToDictionary(Me());
                if (dict["id"]!=null)
                {
                    return userId = dict["id"].ToString();
                }
            }
            return userId;
        }
            set { userId = value; }
        }

        public Dictionary<string, object> ResponseToDictionary(string response)
        {
            var responseValues = HttpUtility.ParseQueryString(response);
            return responseValues.AllKeys.ToDictionary<string, string, object>(responseValue => responseValue,
                                                                               responseValue =>
                                                                               responseValues[responseValue]);
        }

        private void GetDataFromResponse(string response)
        {
            var responseDict = ResponseToDictionary(response);
            if (!responseDict.ContainsKey("access_token"))
            {
                throw new AuthenticationException(response);
            }
            Token = responseDict["access_token"].ToString();
            int expireSeconds;
            if (responseDict.ContainsKey("expires") && int.TryParse(responseDict["expires"].ToString(), out expireSeconds))
            {
                TokenExpires = DateTime.Now.AddSeconds(expireSeconds);
            }
            else
            {
                TokenExpires = DateTime.Now.AddDays(30);
            }
        }


        public string MakeRequest(string requesturl)
        {
            return OAuthClientUtils.MakeRequest(OAuthClientUtils.RequestUrl(BaseRequestUrl, requesturl, Token));
        }

        public DateTime TokenExpires { get; set; }

        public string FriendsList()
        {
            return MakeRequest("/me/friends");
        }
        
        public string Permissions()
        {
            return MakeRequest("/me/permissions");
        }

        public string Me()
        {
            if (!string.IsNullOrEmpty(me))
            {
                return me;
            }
            return me = MakeRequest("/me");
        }

        public string FirstName
        {
            get
            {
                var dict = OAuthClientUtils.JsonToDictionary(Me());
                if (dict.ContainsKey("first_name"))
                {
                    return dict["first_name"].ToString();
                }
                return string.Empty;
            }
        }

        public string LastName
        {
            get
            {
                var dict = OAuthClientUtils.JsonToDictionary(Me());
                if (dict.ContainsKey("last_name"))
                {
                    return dict["last_name"].ToString();
                }
                return string.Empty;
            }
        }

        public DateTime? Birthdate
        {
            get
            {
                var dict = OAuthClientUtils.JsonToDictionary(Me());
                DateTime bday;
                if (dict.ContainsKey("birthday") && DateTime.TryParse((string)dict["birthday"], out bday))
                {
                    return bday;
                }
                return null;
            }
        }

        public string PhotoUrl
        {
            get { return BaseRequestUrl + "/" + UserId + "/picture?type=large"; }
        }

        public Gender Gender
        {
            get
            {
                var dict = OAuthClientUtils.JsonToDictionary(Me());
                if (dict.ContainsKey("gender"))
                {
                    return (string)dict["gender"] == "male" ? Gender.Male : Gender.Female;
                }
                return Gender.Any;
            }
        }
    }
}
