namespace GitTools.IssueTrackers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class IssueTrackerBase : IIssueTracker
    {
        public IssueTrackerBase(IIssueTrackerContext issueTrackerContext)
        {
            IssueTrackerContext = issueTrackerContext;
        }

        protected IIssueTrackerContext IssueTrackerContext { get; private set; }

        public abstract Task<IEnumerable<Issue>> GetIssuesAsync(IssueTrackerFilter filter);
    }
}