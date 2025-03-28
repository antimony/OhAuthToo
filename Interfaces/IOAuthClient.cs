﻿using System;
using System.Threading.Tasks;
using OhAuthToo.Utils;

namespace OhAuthToo.Interfaces
{
    public interface IOAuthClient
    {
        string ClientName { get; }
        string Response { set; }
        string? Token { get; set; }
        string? UserId { get; set; }
        string BaseRequestUrl { get; }
        string MakeRequest(string requesturl);
        Task<string> MakeRequestAsync(string requesturl);
        DateTime TokenExpires { get; set; }
        string FriendsList();
        Task<string> FriendsListAsync();
        string Permissions();
        Task<string> PermissionsAsync();
        string Me();
        Task<string> MeAsync();
        string FirstName { get; }
        string LastName { get; }
        DateTime? Birthdate { get; }
        string PhotoUrl { get; }
        Gender Gender { get; }
    }
}
