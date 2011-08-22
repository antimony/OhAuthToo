
namespace OhAuthToo.ConcreteAuthorizers
{
    public class VkontakteAuthorizer:OAuthAuthorizer
    {
        public override string ClientName
        {
            get { return "Vkontakte"; }
        }

        protected override string AuthorizeUri
        {
            get { return "http://api.vkontakte.ru/oauth/authorize"; }
        }

        protected override string TokenUri
        {
            get { return "https://api.vkontakte.ru/oauth/access_token"; }
        }

    }
}
