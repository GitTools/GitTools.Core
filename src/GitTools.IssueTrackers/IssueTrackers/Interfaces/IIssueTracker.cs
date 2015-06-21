namespace GitTools.IssueTrackers
{
    using System;
    using System.Collections.Generic;
    using Octokit;

    public interface IIssueTracker
    {
        IEnumerable<Issue> GetIssues(IssueTrackerFilter filter);
    }
}