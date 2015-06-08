namespace GitTools.IssueTrackers
{
    public class IssueTrackerContext : IIssueTrackerContext
    {
        public IssueTrackerContext()
        {
            Authentication = new AuthenticationContext();
        }

        public string Server { get; set; }

        public string ProjectId { get; set; }

        public IAuthenticationContext Authentication { get; private set; }
    }
}