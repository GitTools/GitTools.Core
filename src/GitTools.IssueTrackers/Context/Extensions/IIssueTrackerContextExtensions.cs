namespace GitTools.IssueTrackers
{
    using Logging;

    public static class IIssueTrackerContextExtensions
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        // TODO: Use IValidationContext
        public static bool Validate(this IIssueTrackerContext context)
        {
            if (string.IsNullOrWhiteSpace(context.ProjectId))
            {
                Log.Error("IssueTracker.ProjectId is required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(context.Server))
            {
                Log.Error("IssueTracker.Server is required");
                return false;
            }

            return true;
        }

        public static IssueTracker? GetIssueTracker(this IIssueTrackerContext context)
        {
            var server = string.IsNullOrWhiteSpace(context.Server) ? string.Empty : context.Server.ToLower();

            // TODO: implement more detections

            if (server.Contains("bitbucket"))
            {
                return IssueTracker.BitBucket;
            }

            if (server.Contains("github"))
            {
                return IssueTracker.GitHub;
            }

            if (server.Contains("atlassian"))
            {
                return IssueTracker.Jira;
            }

            //if (context.IssueTracker is JiraContext)
            //{
            //    return IssueTracker.Jira;
            //}

            //if (context.IssueTracker is YouTrackContext)
            //{
            //    return IssueTracker.YouTrack;
            //}

            //return null;

            return IssueTracker.Jira;
        }
    }
}