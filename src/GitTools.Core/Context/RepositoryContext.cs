namespace GitTools
{
    public class RepositoryContext : IRepositoryContext
    {
        public string Directory { get; set; }

        public string Branch { get; set; }

        public string Url { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}