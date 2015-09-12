namespace GitTools
{
    public class RepositoryContext : Disposable, IRepositoryContext
    {
        public RepositoryContext()
        {
            Authentication = new AuthenticationContext();
        }

        public string Directory { get; set; }

        public string Branch { get; set; }

        public string Url { get; set; }

        public IAuthenticationContext Authentication { get; private set; }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            var authentication = Authentication;
            if (authentication != null)
            {
                authentication.Dispose();
            }
        }
    }
}