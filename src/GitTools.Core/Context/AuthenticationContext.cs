namespace GitTools
{
    public class AuthenticationContext : IAuthenticationContext
    {
        public AuthenticationContext()
        {
        }

        public AuthenticationContext(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public AuthenticationContext(string token)
        {
            Token = token;
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}