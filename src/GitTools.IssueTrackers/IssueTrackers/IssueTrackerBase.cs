namespace GitTools.IssueTrackers
{
    using System;
    using System.Collections.Generic;

    public abstract class IssueTrackerBase : IIssueTracker
    {
        public IssueTrackerBase(IIssueTrackerContext issueTrackerContext)
        {
            IssueTrackerContext = issueTrackerContext;
        }

        protected IIssueTrackerContext IssueTrackerContext { get; private set; }

        public abstract IEnumerable<Issue> GetIssues(IssueTrackerFilter filter);
    }
}