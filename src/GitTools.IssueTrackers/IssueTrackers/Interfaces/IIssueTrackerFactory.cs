namespace GitTools.IssueTrackers
{
    public interface IIssueTrackerFactory
    {
        IIssueTracker CreateIssueTracker(IIssueTrackerContext issueTrackerContext);
    }
}