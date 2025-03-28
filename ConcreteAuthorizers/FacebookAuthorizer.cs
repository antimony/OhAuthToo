﻿namespace OhAuthToo.ConcreteAuthorizers
{
    public class FacebookAuthorizer : OAuthAuthorizer
    {
        public override string ClientName
        {
            get { return "Facebook"; }
        }

        protected override string? AuthorizeUri
        {
            get { return "https://www.facebook.com/dialog/oauth"; }
            set { base.AuthorizeUri = value; }
        }

        protected override string? TokenUri
        {
            get { return "https://graph.facebook.com/oauth/access_token"; }
            set { base.TokenUri = value; }
        }
    }
}
