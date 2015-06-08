namespace GitTools.IssueTrackers.Jira
{
    using System.Collections.Generic;
    using global::Jira.SDK;
    using Logging;

    public class JiraIssueTracker : IssueTrackerBase
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly IIssueTrackerContext _issueTrackerContext;

        public JiraIssueTracker(IIssueTrackerContext issueTrackerContext)
        {
            _issueTrackerContext = issueTrackerContext;
        }

        public override IEnumerable<Issue> GetIssues(string filter)
        {
            var jira = new Jira();

            Log.DebugFormat("Connecting to Jira server '{0}'", _issueTrackerContext.Server);

            jira.Connect(_issueTrackerContext.Server, _issueTrackerContext.Authentication.Username, _issueTrackerContext.Authentication.Password);

            var issues = new List<Issue>();

            foreach (var issue in jira.SearchIssues(filter))
            {
                var gitIssue = new Issue(issue.Key)
                {
                    IssueType = IssueType.Issue,
                    Title = issue.Summary,
                    Description = issue.Description
                };

                issues.Add(gitIssue);
            }

            return issues;
        }
    }
}