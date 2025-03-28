﻿using OhAuthToo.Interfaces;
using OhAuthToo.Utils;
using System;
using System.Threading.Tasks;

namespace OhAuthToo.ConcreteClients
{
    public class VkontakteClient : IOAuthClient
    {
        private string? me;
        public string BaseRequestUrl { get { return "https://api.vkontakte.ru/method"; } }
        
        private void GetDataFromResponse(string response)
        {
            var responseDict = OAuthClientUtils.JsonToDictionary(response);
            if (responseDict == null || !responseDict.ContainsKey("access_token"))
            {
                throw new Exception(response);
            }
            Token = responseDict["access_token"].ToString();
            UserId = responseDict["user_id"].ToString();
            
            if (responseDict.TryGetValue("expires_in", out var expiresObj) && 
                expiresObj != null)
            {
                if (expiresObj is int expiresInt)
                {
                    TokenExpires = DateTime.Now.AddSeconds(expiresInt);
                }
                else if (int.TryParse(expiresObj.ToString(), out int expiresValue))
                {
                    TokenExpires = DateTime.Now.AddSeconds(expiresValue);
                }
                else
                {
                    TokenExpires = DateTime.Now.AddDays(30); // Default
                }
            }
            else
            {
                TokenExpires = DateTime.Now.AddDays(30); // Default
            }
        }

        public string? Code { get; set; }

        public string ClientName
        {
            get { return "Vkontakte"; }
        }

        public string Response
        {
            set { GetDataFromResponse(value); }
        }

        public string? Token { get; set; }
        public string? UserId { get; set; }
        
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
            if (string.IsNullOrEmpty(UserId))
            {
                throw new InvalidOperationException("UserId must be set before getting friends list");
            }
            return MakeRequest("/friends.get?fields=first_name,last_name,sex,photo&uid=" + UserId);
        }

        public async Task<string> FriendsListAsync()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                throw new InvalidOperationException("UserId must be set before getting friends list");
            }
            return await MakeRequestAsync("/friends.get?fields=first_name,last_name,sex,photo&uid=" + UserId);
        }

        public string Permissions()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                throw new InvalidOperationException("UserId must be set before getting permissions");
            }
            return MakeRequest("/getUserSettings?uid=" + UserId);
        }

        public async Task<string> PermissionsAsync()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                throw new InvalidOperationException("UserId must be set before getting permissions");
            }
            return await MakeRequestAsync("/getUserSettings?uid=" + UserId);
        }

        public string Me()
        {
            if (!string.IsNullOrEmpty(me))
            {
                return me;
            }
            
            if (string.IsNullOrEmpty(UserId))
            {
                throw new InvalidOperationException("UserId must be set before getting profile");
            }
            
            string mearray = MakeRequest("/getProfiles?fields=first_name,last_name,sex,photo&uids=" + UserId);
            if (string.IsNullOrEmpty(mearray) || mearray == "[]" || mearray.Contains("error"))
            {
                return mearray;
            }
            int start = mearray.IndexOf('[');
            int end = mearray.IndexOf(']');
            if ((start + 1) >= (end - 1))
            {
                return string.Empty;
            }
            return me = mearray.Substring(start + 1, end - start - 1);
        }

        public async Task<string> MeAsync()
        {
            if (!string.IsNullOrEmpty(me))
            {
                return me;
            }
            
            if (string.IsNullOrEmpty(UserId))
            {
                throw new InvalidOperationException("UserId must be set before getting profile");
            }
            
            string mearray = await MakeRequestAsync("/getProfiles?fields=first_name,last_name,sex,photo&uids=" + UserId);
            if (string.IsNullOrEmpty(mearray) || mearray == "[]" || mearray.Contains("error"))
            {
                return mearray;
            }
            int start = mearray.IndexOf('[');
            int end = mearray.IndexOf(']');
            if ((start + 1) >= (end - 1))
            {
                return string.Empty;
            }
            return me = mearray.Substring(start + 1, end - start - 1);
        }

        public string FirstName
        {
            get { 
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
                if (dict != null && dict.TryGetValue("bdate", out var bdateObj) && 
                    bdateObj != null && 
                    DateTime.TryParse(bdateObj.ToString(), out DateTime bday))
                {
                    return bday;
                }
                return null;
            }
        }

        public string PhotoUrl
        {
            get 
            {
                var dict = OAuthClientUtils.JsonToDictionary(Me());
                if (dict != null && dict.TryGetValue("photo", out var photoObj) && photoObj != null)
                {
                    return photoObj.ToString();
                }
                return string.Empty;
            }
        }

        public Gender Gender
        {
            get 
            { 
                var dict = OAuthClientUtils.JsonToDictionary(Me());
                if (dict != null && dict.TryGetValue("sex", out var sexObj) && sexObj != null)
                {
                    return sexObj.ToString() == "male" ? Gender.Male : Gender.Female;
                }
                return Gender.Any;
            }
        }
    }
}
