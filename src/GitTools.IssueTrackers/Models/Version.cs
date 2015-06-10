namespace GitTools.IssueTrackers
{
    using System;

    public class Version
    {
        public string Name { get; set; }

        public DateTimeOffset? ReleaseDate { get; set; }

        public bool IsReleased { get; set; }
    }
}