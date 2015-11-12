namespace GitTools
{
    using System;
    using LibGit2Sharp;
    using Logging;

    public static class LibGitExtensions
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public static DateTimeOffset When(this Commit commit)
        {
            return commit.Committer.When;
        }

        public static bool IsDetachedHead(this Branch branch)
        {
            return branch.CanonicalName.Equals("(no branch)", StringComparison.OrdinalIgnoreCase);
        }
    }
}