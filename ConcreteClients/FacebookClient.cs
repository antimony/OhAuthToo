﻿using OhAuthToo.Interfaces;
using OhAuthToo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web;

namespace OhAuthToo.ConcreteClients
{
    public class FacebookClient : IOAuthClient
    {
        private string? userId;
        private string? me;
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

        public string? Token { get; set; }
        public string? UserId
        {
            get
            {
                if (string.IsNullOrEmpty(userId))
                {
                    var dict = OAuthClientUtils.JsonToDictionary(Me());
                    if (dict != null && dict.TryGetValue("id", out var id) && id != null)
                    {
                        return userId = id.ToString();
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
            if (responseDict.TryGetValue("expires", out var expiresObj) && 
                expiresObj != null && 
                int.TryParse(expiresObj.ToString(), out int expireSeconds))
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
            if (string.IsNullOrEmpty(Token))
            {
                throw new InvalidOperationException("Token must be set before making requests");
            }
            return OAuthClientUtils.MakeRequest(OAuthClientUtils.RequestUrl(BaseRequestUrl, requesturl, Token));
        }

        public async Task<string> MakeRequestAsync(string requesturl)
        {
            if (string.IsNullOrEmpty(Token))
            {
                throw new InvalidOperationException("Token must be set before making requests");
            }
            return await OAuthClientUtils.MakeRequestAsync(OAuthClientUtils.RequestUrl(BaseRequestUrl, requesturl, Token));
        }

        public DateTime TokenExpires { get; set; }

        public string FriendsList()
        {
            return MakeRequest("/me/friends");
        }

        public Task<string> FriendsListAsync()
        {
            return MakeRequestAsync("/me/friends");
        }
        
        public string Permissions()
        {
            return MakeRequest("/me/permissions");
        }

        public Task<string> PermissionsAsync()
        {
            return MakeRequestAsync("/me/permissions");
        }

        public string Me()
        {
            if (!string.IsNullOrEmpty(me))
            {
                return me;
            }
            return me = MakeRequest("/me");
        }

        public async Task<string> MeAsync()
        {
            if (!string.IsNullOrEmpty(me))
            {
                return me;
            }
            return me = await MakeRequestAsync("/me");
        }

        public string FirstName
        {
            get
            {
                var dict = OAuthClientUtils.JsonToDictionary(Me());
                if (dict != null && dict.TryGetValue("first_name", out var firstName) && firstName != null)
                {
                    return firstName.ToString();
                }
                return string.Empty;
            }
        }

        public string LastName
        {
            get
            {
                var dict = OAuthClientUtils.JsonToDictionary(Me());
                if (dict != null && dict.TryGetValue("last_name", out var lastName) && lastName != null)
                {
                    return lastName.ToString();
                }
                return string.Empty;
            }
        }

        public DateTime? Birthdate
        {
            get
            {
                var dict = OAuthClientUtils.JsonToDictionary(Me());
                if (dict != null && dict.TryGetValue("birthday", out var birthdayObj) && 
                    birthdayObj != null && 
                    DateTime.TryParse(birthdayObj.ToString(), out DateTime bday))
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
                if (dict != null && dict.TryGetValue("gender", out var genderObj) && genderObj != null)
                {
                    return genderObj.ToString() == "male" ? Gender.Male : Gender.Female;
                }
                return Gender.Any;
            }
        }
    }
}
