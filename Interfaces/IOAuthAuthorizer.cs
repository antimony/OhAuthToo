namespace OhAuthToo.Interfaces
{
    public interface IOAuthAuthorizer
    {
        string ClientName { get; }
        string ClientId { get; set; }
        string RedirectUri { get; set; }
        string Scope { get; set; }
        string ClientSecret { get; set; }
        string CodeRequestUri { get; }
        string GetAuthorizationResponse(string code);
        void GetAuthorizationResponseAsync(string code);
    }
}
