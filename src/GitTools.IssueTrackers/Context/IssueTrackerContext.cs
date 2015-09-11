namespace GitTools.IssueTrackers
{
    public class IssueTrackerContext : Disposable, IIssueTrackerContext
    {
        public IssueTrackerContext()
        {
            Authentication = new AuthenticationContext();
        }

        public string Server { get; set; }

        public string DiffUrlFormat { get; set; }

        public string ProjectId { get; set; }

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