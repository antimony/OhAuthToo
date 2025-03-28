﻿﻿﻿using System.Threading.Tasks;

namespace OhAuthToo.Interfaces
{
    public interface IOAuthAuthorizer
    {
        string ClientName { get; }
        string? ClientId { get; set; }
        string? RedirectUri { get; set; }
        string? Scope { get; set; }
        string? ClientSecret { get; set; }
        string CodeRequestUri { get; }
        string GetAuthorizationResponse(string code);
        Task<string> GetAuthorizationResponseAsync(string code);
    }
}
