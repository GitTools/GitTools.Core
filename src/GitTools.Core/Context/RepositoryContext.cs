namespace GitTools
{
    public class RepositoryContext : IRepositoryContext
    {
        public RepositoryContext()
        {
            Authentication = new AuthenticationContext();
        }

        public string Directory { get; set; }

        public string Branch { get; set; }

        public string Url { get; set; }

        public IAuthenticationContext Authentication { get; private set; }
    }
}