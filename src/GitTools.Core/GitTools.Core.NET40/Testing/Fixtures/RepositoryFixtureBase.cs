namespace GitTools.Testing
{
    using System;
    using LibGit2Sharp;
    using Logging;

    public abstract class RepositoryFixtureBase : IDisposable
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public readonly IRepository Repository;
        public readonly string RepositoryPath;

        protected RepositoryFixtureBase(Func<string, IRepository> repoBuilder)
        {
            TemporaryFilesContext = new TemporaryFilesContext();

            RepositoryPath = TemporaryFilesContext.GetDirectory("/");
            Repository = repoBuilder(RepositoryPath);
            Repository.Config.Set("user.name", "Test");
            Repository.Config.Set("user.email", "test@email.com");
            IsForTrackedBranchOnly = true;
        }

        protected TemporaryFilesContext TemporaryFilesContext { get; private set; }

        public bool IsForTrackedBranchOnly { private get; set; }

        public virtual void Dispose()
        {
            var temporaryFilesContext = TemporaryFilesContext;
            if (temporaryFilesContext != null)
            {
                temporaryFilesContext.Dispose();
                TemporaryFilesContext = null;
            }

            Repository.Dispose();
        }
    }
}