namespace GitTools.IssueTrackers
{
    public interface IIssueTrackerContext
    {
        string Server { get; set; }

        string ProjectId { get; set; }

        IAuthenticationContext Authentication { get; }
    }
}