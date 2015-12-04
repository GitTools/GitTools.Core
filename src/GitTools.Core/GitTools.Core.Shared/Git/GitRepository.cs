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

            ProjectRootDirectory = repository.Info.WorkingDirectory;
            DotGitDirectory = Path.Combine(ProjectRootDirectory, ".git");
        }

        public IRepository Repository { get; private set; }

        public bool IsDynamic { get; private set; }

        public string DotGitDirectory { get; private set; }

        public string ProjectRootDirectory { get; private set; }

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