namespace GitTools
{
    public interface IRepositoryContext
    {
        IAuthenticationContext Authentication { get; }

        string Directory { get; set; }

        string Branch { get; set; }

        string Url { get; set; }
    }
}