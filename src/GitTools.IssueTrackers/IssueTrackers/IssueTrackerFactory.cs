namespace GitTools.IssueTrackers
{
    using GitHub;
    using Jira;
    using Octokit;

    public class IssueTrackerFactory : IIssueTrackerFactory
    {
        public IIssueTracker CreateIssueTracker(IIssueTrackerContext context)
        {
            switch (context.GetIssueTracker())
            {
                //case IssueTracker.BitBucket:
                //    break;

                case IssueTracker.GitHub:
                    return new GitHubIssueTracker(context);

                case IssueTracker.Jira:
                    return new JiraIssueTracker(context);

                //case IssueTracker.YouTrack:
                //    return new YouTrackIssueTracker(new YouTrackApi(), context);
            }

            return null;
        }
    }
}