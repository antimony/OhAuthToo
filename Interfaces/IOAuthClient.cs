using System;
using OhAuthToo.Utils;

namespace OhAuthToo.Interfaces
{
    public interface IOAuthClient
    {
        string ClientName { get; }
        string Response { set; }
        string Token { get; set; }
        string UserId { get; set; }
        string BaseRequestUrl { get; }
        string MakeRequest(string requesturl);
        DateTime TokenExpires { get; set; }
        string FriendsList();
        string Permissions();
        string Me();
        string FirstName { get; }
        string LastName { get; }
        DateTime? Birthdate { get; }
        string PhotoUrl { get; }
        Gender Gender { get; }
    }
}
