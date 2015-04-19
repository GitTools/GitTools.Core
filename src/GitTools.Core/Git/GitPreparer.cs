namespace GitTools.Git
{
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.Logging;
    using LibGit2Sharp;

    public class GitPreparer : IRepositoryPreparer
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public bool IsPreparationRequired(IRepositoryContext context)
        {
            Argument.IsNotNull(() => context);

            var gitPath = GitDirFinder.TreeWalkForGitDir(context.Directory);
            return string.IsNullOrEmpty(gitPath);
        }

        public string Prepare(IRepositoryContext context, TemporaryFilesContext temporaryFilesContext)
        {
            Argument.IsNotNull(() => context);

            var gitDirectory = temporaryFilesContext.GetDirectory("git");

            // TODO: convert to io service to abstract away IO
            Directory.CreateDirectory(gitDirectory);

            if (!string.IsNullOrWhiteSpace(context.Url))
            {
                gitDirectory = GetGitInfoFromUrl(context, gitDirectory);
            }

            return GitDirFinder.TreeWalkForGitDir(gitDirectory);
        }

        private string GetGitInfoFromUrl(IRepositoryContext context, string gitDirectory)
        {
            gitDirectory = Path.Combine(gitDirectory, ".git");
            if (Directory.Exists(gitDirectory))
            {
                Log.Info("Deleting existing .git folder from '{0}' to force new checkout from url", gitDirectory);

                DeleteHelper.DeleteGitRepository(gitDirectory);
            }

            Log.Info("Retrieving git info from url '{0}'", context.Url);

            Credentials credentials = null;
            if (!string.IsNullOrWhiteSpace(context.Username) && !string.IsNullOrWhiteSpace(context.Password))
            {
                Log.Info("Setting up credentials using name '{0}'", context.Username);

                credentials = new UsernamePasswordCredentials
                {
                    Username = context.Username,
                    Password = context.Password
                };
            }

            var cloneOptions = new CloneOptions
            {
                Checkout = false,
                IsBare = true,
                CredentialsProvider = (url, username, supportedTypes) => credentials
            };

            Repository.Clone(context.Url, gitDirectory, cloneOptions);

            if (!string.IsNullOrWhiteSpace(context.Branch))
            {
                using (var repository = new Repository(gitDirectory))
                {
                    Reference newHead = null;

                    var localReference = GetLocalReference(repository, context.Branch);
                    if (localReference != null)
                    {
                        newHead = localReference;
                    }

                    if (newHead == null)
                    {
                        var remoteReference = GetRemoteReference(repository, context.Branch, context.Url);
                        if (remoteReference != null)
                        {
                            repository.Network.Fetch(context.Url, new[]
                            {
                                string.Format("{0}:{1}", remoteReference.CanonicalName, context.Branch)
                            });

                            newHead = repository.Refs[string.Format("refs/heads/{0}", context.Branch)];
                        }
                    }

                    if (newHead != null)
                    {
                        Log.Info("Switching to branch '{0}'", context.Branch);

                        repository.Refs.UpdateTarget(repository.Refs.Head, newHead);
                    }
                }
            }

            return gitDirectory;
        }

        private static Reference GetLocalReference(Repository repository, string branchName)
        {
            var targetBranchName = branchName.GetCanonicalBranchName();

            return repository.Refs.FirstOrDefault(localRef => string.Equals(localRef.CanonicalName, targetBranchName));
        }

        private static DirectReference GetRemoteReference(Repository repository, string branchName, string repositoryUrl)
        {
            var targetBranchName = branchName.GetCanonicalBranchName();
            var remoteReferences = repository.Network.ListReferences(repositoryUrl);

            return remoteReferences.FirstOrDefault(remoteRef => string.Equals(remoteRef.CanonicalName, targetBranchName));
        }
    }
}