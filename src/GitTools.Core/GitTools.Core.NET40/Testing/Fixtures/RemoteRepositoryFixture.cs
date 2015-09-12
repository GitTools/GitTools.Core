namespace GitTools.Testing
{
    using System;
    using LibGit2Sharp;

    public class RemoteRepositoryFixture : RepositoryFixtureBase
    {
        public IRepository LocalRepository;
        public string LocalRepositoryPath;

        public RemoteRepositoryFixture()
            : base(CreateNewRepository)
        {
            CloneRepository();
        }

        private static IRepository CreateNewRepository(string path)
        {
            LibGit2Sharp.Repository.Init(path);
            Console.WriteLine("Created git repository at '{0}'", path);

            var repo = new Repository(path);
            repo.MakeCommits(5);
            return repo;
        }

        private void CloneRepository()
        {
            LocalRepositoryPath = TemporaryFilesContext.GetDirectory("/");
            LibGit2Sharp.Repository.Clone(RepositoryPath, LocalRepositoryPath);
            LocalRepository = new Repository(LocalRepositoryPath);
        }

        public override void Dispose()
        {
            LocalRepository.Dispose();

            base.Dispose();
        }
    }
}