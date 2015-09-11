namespace GitTools.IssueTrackers
{
    using System;

    public interface IIssueTrackerContext : IDisposable
    {
        string Server { get; set; }

        string DiffUrlFormat { get; set; }

        string ProjectId { get; set; }

        IAuthenticationContext Authentication { get; }
    }
}