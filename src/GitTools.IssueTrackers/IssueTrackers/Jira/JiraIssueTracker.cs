namespace GitTools.IssueTrackers.Jira
{
    using System.Collections.Generic;
    using System.Linq;
    using Atlassian.Jira;
    using Logging;
    using Issue = IssueTrackers.Issue;
    using IssueType = IssueTrackers.IssueType;

    public class JiraIssueTracker : IssueTrackerBase
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private static readonly HashSet<string> KnownClosedStatuses = new HashSet<string>(new [] { "resolved", "closed", "done" }); 

        private readonly IIssueTrackerContext _issueTrackerContext;

        public JiraIssueTracker(IIssueTrackerContext issueTrackerContext)
        {
            _issueTrackerContext = issueTrackerContext;
        }

        public override IEnumerable<Issue> GetIssues(string filter = null, bool includeOpen = true, bool includeClosed = true)
        {
            Log.DebugFormat("Connecting to Jira server '{0}'", _issueTrackerContext.Server);

            var jira = new Jira(_issueTrackerContext.Server, _issueTrackerContext.Authentication.Username, _issueTrackerContext.Authentication.Password);
            jira.MaxIssuesPerRequest = 500;

            Log.Debug("Retrieving statuses");

            var openedStatuses = GetOpenedStatuses(jira);
            var closedStatuses = GetClosedStatuses(jira);

            filter = PrepareFilter(filter, openedStatuses, closedStatuses, includeOpen, includeClosed);

            var issues = new List<Issue>();

            Log.DebugFormat("Searching for issues using filter '{0}'", filter);

            foreach (var issue in jira.GetIssuesFromJql(filter))
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
                        Value = fixVersion.Name,
                        ReleaseDate = fixVersion.ReleasedDate,
                        IsReleased = fixVersion.IsReleased
                    });
                }

                issues.Add(gitIssue);
            }

            Log.DebugFormat("Found '{0}' issues using filter '{1}'", issues.Count, filter);

            return issues;
        }

        private string PrepareFilter(string filter, IEnumerable<IssueStatus> openedStatuses, IEnumerable<IssueStatus> closedStatuses, 
            bool includeOpen = true, bool includeClosed = true)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                filter = "";
            }
            else
            {
                filter += " AND ";
            }

            if (includeOpen && includeClosed)
            {
                // no need to filter anything
            }

            if (!includeOpen && !includeClosed)
            {
                throw new GitToolsException("Cannot exclude both open and closed issues, nothing will be returned");
            }

            if (includeOpen)
            {
                filter += string.Format("status in ({0})", string.Join(", ", openedStatuses.Select(x => string.Format("\"{0}\"", x))));
            }

            if (includeClosed)
            {
                filter += string.Format("status in ({0})", string.Join(", ", closedStatuses.Select(x => string.Format("\"{0}\"", x))));
            }

            return filter;
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