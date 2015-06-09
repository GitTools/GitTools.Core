namespace GitTools.IssueTrackers
{
    using Jira;

    public class IssueTrackerFactory : IIssueTrackerFactory
    {
        public IIssueTracker CreateIssueTracker(IIssueTrackerContext context)
        {
            switch (context.GetIssueTracker())
            {
                //case IssueTracker.BitBucket:
                //    break;

                //case IssueTracker.GitHub:
                //    return new GitHubIssueTracker(repository, () =>
                //    {
                //        var gitHubClient = new GitHubClient(new ProductHeaderValue("GitReleaseNotes"));
                //        if (context.IssueTracker.Token != null)
                //        {
                //            gitHubClient.Credentials = new Octokit.Credentials(context.IssueTracker.Token);
                //        }

                //        return gitHubClient;
                //    }, context);

                case IssueTracker.Jira:
                    return new JiraIssueTracker(context);

                //case IssueTracker.YouTrack:
                //    return new YouTrackIssueTracker(new YouTrackApi(), context);
            }

            return null;
        }
    }
}