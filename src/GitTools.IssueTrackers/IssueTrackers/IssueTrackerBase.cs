namespace GitTools.IssueTrackers
{
    using System.Collections.Generic;

    public abstract class IssueTrackerBase : IIssueTracker
    {
        public abstract IEnumerable<Issue> GetIssues(string filter);
    }
}