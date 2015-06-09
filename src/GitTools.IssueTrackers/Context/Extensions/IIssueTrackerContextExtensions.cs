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
            return IssueTracker.Jira;

            // TODO: implement more detections

            //if (context.IssueTracker is BitBucketContext)
            //{
            //    return IssueTracker.BitBucket;
            //}

            //if (context.IssueTracker is GitHubContext)
            //{
            //    return IssueTracker.GitHub;
            //}

            //if (context.IssueTracker is JiraContext)
            //{
            //    return IssueTracker.Jira;
            //}

            //if (context.IssueTracker is YouTrackContext)
            //{
            //    return IssueTracker.YouTrack;
            //}

            //return null;
        }
    }
}