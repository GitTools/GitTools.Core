namespace GitTools.IssueTrackers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IIssueTracker
    {
        Task<IEnumerable<Issue>> GetIssuesAsync(IssueTrackerFilter filter);
    }
}