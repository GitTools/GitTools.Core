namespace GitTools.IssueTrackers
{
    using System;
    using System.Collections.Generic;

    public interface IIssueTracker
    {
        IEnumerable<Issue> GetIssues(string filter = null, bool includeOpen = true, bool includeClosed = true, DateTimeOffset? since = null);
    }
}