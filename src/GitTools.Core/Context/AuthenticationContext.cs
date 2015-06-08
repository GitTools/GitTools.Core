namespace GitTools
{
    using System.Security;

    public class AuthenticationContext : Disposable, IAuthenticationContext
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

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            // TODO: dipose secure strings if needed
        }
    }
}