﻿namespace GitTools.IssueTrackers
{
    using System;

    public class Issue
    {
        public Issue(string id, DateTimeOffset dateClosed)
        {
            Id = id;
            DateClosed = dateClosed;
        }

        public string Id { get; private set; }
        public DateTimeOffset DateClosed { get; set; }

        public IssueType IssueType { get; set; }
        public Uri HtmlUrl { get; set; }
        public string Title { get; set; }
        public string[] Labels { get; set; }
        //public Contributor[] Contributors { get; set; }
    }
}