namespace GitTools.IssueTrackers
{
    using System;

    public class Issue
    {
        public Issue(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }

        public DateTimeOffset DateClosed { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        
        public string[] Labels { get; set; }

        public IssueType IssueType { get; set; }

        public string Url { get; set; }


        //public Uri HtmlUrl { get; set; }

        //public Contributor[] Contributors { get; set; }
    }
}