namespace GitTools.IssueTrackers
{
    using System.Collections.Generic;

    public interface IIssueTracker
    {
        IEnumerable<Issue> GetIssues();

        //IEnumerable<OnlineIssue> GetClosedIssues(IIssueTrackerContext context, DateTimeOffset? since);
    }
}