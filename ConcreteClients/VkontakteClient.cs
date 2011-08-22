using OhAuthToo.Interfaces;
using OhAuthToo.Utils;
using System;

namespace OhAuthToo.ConcreteClients
{
    public class VkontakteClient:IOAuthClient
    {
        private string me;
        public string BaseRequestUrl { get { return "https://api.vkontakte.ru/method"; } }
        
        private void GetDataFromResponse(string response)
        {
            var responseDict = OAuthClientUtils.JsonToDictionary(response);
            if (!responseDict.ContainsKey("access_token"))
            {
                throw new Exception(response);
            }
            Token = responseDict["access_token"].ToString();
            UserId = responseDict["user_id"].ToString();
            TokenExpires = DateTime.Now.AddSeconds((int)responseDict["expires_in"]);
        }

        public string Code { get; set; }

        public string ClientName
        {
            get { return "Vkontakte"; }
        }

        public string Response
        {
            set { GetDataFromResponse(value); }
        }

        public string Token { get; set; }
        public string UserId { get; set; }
        public string MakeRequest(string requesturl)
        {
            return OAuthClientUtils.MakeRequest(OAuthClientUtils.RequestUrl(BaseRequestUrl, requesturl, Token));
        }

        public DateTime TokenExpires { get; set; }
      
        public string FriendsList()
        {
            return MakeRequest("/friends.get?fields=first_name,last_name,sex,photo&uid=" + UserId);
        }

        public string Permissions()
        {
            return MakeRequest("/getUserSettings?uid=" + UserId);
        }

        public string Me()
        {
            if (!string.IsNullOrEmpty(me))
            {
                return me;
            }
            string mearray = MakeRequest("/getProfiles?fields=first_name,last_name,sex,photo&uids=" + UserId);
            if (string.IsNullOrEmpty(mearray) || mearray == "[]" ||mearray.Contains("error"))
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
                return dict["first_name"].ToString();
            } 
        }

        public string LastName
        {
            get
            {
                var dict = OAuthClientUtils.JsonToDictionary(Me());
                return dict["last_name"].ToString();
            }
        }

        public DateTime? Birthdate
        {
            get
            {
                var dict = OAuthClientUtils.JsonToDictionary(Me());
                DateTime bday;
                if (dict.ContainsKey("bdate") && DateTime.TryParse((string)dict["bdate"], out bday))
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
                if (dict.ContainsKey("photo"))
                {
                    return dict["photo"].ToString();
                }
                return string.Empty;
            }
        }

        public Gender Gender
        {
            get 
            { 
                var dict = OAuthClientUtils.JsonToDictionary(Me());
                if (dict["sex"]!=null)
                {
                    return (string)dict["sex"] == "male" ? Gender.Male : Gender.Female;
                }
                return Gender.Any;
            }
        }
    }
}
