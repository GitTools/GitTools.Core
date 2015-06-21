namespace GitTools.IssueTrackers
{
    using System;

    public class IssueTrackerFilter
    {
        public IssueTrackerFilter()
        {
            IncludeOpen = true;
            IncludeClosed = true;
        }

        public string Filter { get; set; }

        public bool IncludeOpen { get; set; }

        public bool IncludeClosed { get; set; }

        public DateTimeOffset? Since { get; set; }
    }
}