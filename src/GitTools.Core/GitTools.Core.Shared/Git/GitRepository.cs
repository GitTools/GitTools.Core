namespace GitTools.Git
{
    using System.IO;
    using LibGit2Sharp;

    public class GitRepository : Disposable
    {
        public GitRepository(IRepository repository, bool isDynamic)
        {
            Repository = repository;
            IsDynamic = isDynamic;
        }

        public IRepository Repository { get; private set; }

        public bool IsDynamic { get; private set; }

        // TODO: Consider using properties
        public string GetDotGitDirectory()
        {
            var rootDirectory = GetProjectRootDirectory();
            var directory = Path.Combine(rootDirectory, ".git");
            return directory;
        }

        // TODO: Consider using properties
        public string GetProjectRootDirectory()
        {
            return Repository.Info.WorkingDirectory;
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            if (Repository != null)
            {
                Repository.Dispose();
                Repository = null;
            }
        }
    }
}