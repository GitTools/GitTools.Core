namespace GitTools
{
    public interface IAuthenticationContext
    {
        string Username { get; set; }

        string Password { get; set; }
    }
}