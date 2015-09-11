namespace GitTools.IssueTrackers.Jira
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Atlassian.Jira;
    using Logging;
    using Issue = IssueTrackers.Issue;
    using IssueType = IssueTrackers.IssueType;
    using Version = IssueTrackers.Version;

    public class JiraIssueTracker : IssueTrackerBase
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private static readonly HashSet<string> KnownClosedStatuses = new HashSet<string>(new [] { "resolved", "closed", "done" }); 

        public JiraIssueTracker(IIssueTrackerContext issueTrackerContext)
            : base(issueTrackerContext)
        {
        }

        public override async Task<IEnumerable<Issue>> GetIssuesAsync(IssueTrackerFilter filter)
        {
            Log.DebugFormat("Connecting to Jira server '{0}'", IssueTrackerContext.Server);

            var jira = new Jira(IssueTrackerContext.Server, IssueTrackerContext.Authentication.Username, IssueTrackerContext.Authentication.Password);
            jira.MaxIssuesPerRequest = 500;

            Log.Debug("Retrieving statuses");

            var openedStatuses = GetOpenedStatuses(jira);
            var closedStatuses = GetClosedStatuses(jira);

            var finalFilter = PrepareFilter(filter, openedStatuses, closedStatuses);

            var issues = new List<Issue>();

            Log.DebugFormat("Searching for issues using filter '{0}'", filter);

            foreach (var issue in jira.GetIssuesFromJql(finalFilter))
            {
                var gitIssue = new Issue(issue.Key.Value)
                {
                    IssueType = IssueType.Issue,
                    Title = issue.Summary,
                    Description = issue.Description,
                    DateCreated = issue.Created
                };

                if (closedStatuses.Any(x => string.Equals(x.Id, issue.Status.Id)))
                {
                    gitIssue.DateClosed = issue.GetResolutionDate();
                }

                foreach (var fixVersion in issue.FixVersions)
                {
                    gitIssue.FixVersions.Add(new Version
                    {
                        Name = fixVersion.Name,
                        ReleaseDate = fixVersion.ReleasedDate,
                        IsReleased = fixVersion.IsReleased
                    });
                }

                issues.Add(gitIssue);
            }

            Log.DebugFormat("Found '{0}' issues using filter '{1}'", issues.Count, filter);

            return issues;
        }

        private string PrepareFilter(IssueTrackerFilter filter, IEnumerable<IssueStatus> openedStatuses, IEnumerable<IssueStatus> closedStatuses)
        {
            var finalFilter = string.Empty;
            if (!string.IsNullOrWhiteSpace(filter.Filter))
            {
                finalFilter = filter.Filter + " AND ";
            }

            if (filter.IncludeOpen && filter.IncludeClosed)
            {
                // no need to filter anything
            }

            if (!filter.IncludeOpen && !filter.IncludeClosed)
            {
                throw new GitToolsException("Cannot exclude both open and closed issues, nothing will be returned");
            }

            if (filter.IncludeOpen)
            {
                finalFilter += string.Format("status in ({0})", string.Join(", ", openedStatuses.Select(x => string.Format("\"{0}\"", x))));
            }

            if (filter.IncludeClosed)
            {
                finalFilter += string.Format("status in ({0})", string.Join(", ", closedStatuses.Select(x => string.Format("\"{0}\"", x))));
            }

            return finalFilter;
        }

        private List<IssueStatus> GetOpenedStatuses(Jira jira)
        {
            var issueStatuses = jira.GetIssueStatuses().ToList();

            return (from issueStatus in issueStatuses
                    where !KnownClosedStatuses.Contains(issueStatus.Name.ToLower())
                    select issueStatus).ToList();
        } 

        private List<IssueStatus> GetClosedStatuses(Jira jira)
        {
            var issueStatuses = jira.GetIssueStatuses().ToList();

            return (from issueStatus in issueStatuses
                    where KnownClosedStatuses.Contains(issueStatus.Name.ToLower())
                    select issueStatus).ToList();
        }
    }
}