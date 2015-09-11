namespace GitTools.IssueTrackers
{
    using System;
    using System.Collections.Generic;

    public class Issue
    {
        public Issue(string id)
        {
            Id = id;

            FixVersions = new List<Version>();
            Contributors = new Contributor[0];
        }

        public string Id { get; private set; }

        public DateTimeOffset? DateCreated { get; set; }

        public DateTimeOffset? DateClosed { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        
        public string[] Labels { get; set; }

        public IssueType IssueType { get; set; }

        public string Url { get; set; }

        public List<Version> FixVersions { get; private set; } 

        //public Uri HtmlUrl { get; set; }

        public Contributor[] Contributors { get; set; }
    }
}