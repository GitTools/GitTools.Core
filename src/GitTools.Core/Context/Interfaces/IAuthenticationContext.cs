namespace GitTools
{
    using System;
    using System.Security;

    public interface IAuthenticationContext : IDisposable
    {
        string Username { get; set; }

        string Password { get; set; }

        string Token { get; set; }
    }
}