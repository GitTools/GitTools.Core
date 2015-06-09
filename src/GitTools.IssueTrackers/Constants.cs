namespace GitTools.IssueTrackers
{
    public enum IssueType
    {
        PullRequest,

        Issue
    }

    public enum IssueTracker
    {
        BitBucket,
        GitHub,
        Jira,
        YouTrack,

        //TODO Tfs

        Unknown,
    }
}