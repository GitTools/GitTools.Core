namespace GitTools
{
    public interface IRepositoryContext : IAuthenticationContext
    {
        string Directory { get; set; }

        string Branch { get; set; }

        string Url { get; set; }
    }
}