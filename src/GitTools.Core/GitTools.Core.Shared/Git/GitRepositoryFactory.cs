namespace GitTools.Git
{
    using System;
    using System.IO;
    using System.Linq;
    using LibGit2Sharp;
    using Logging;

    public static class GitRepositoryFactory
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Creates the repository based on the repository info. If the <see cref="RepositoryInfo.Directory"/> points 
        /// to a valid directory, it will be used as the source for the git repository. Otherwise this method will create
        /// a dynamic repository based on the url and authentication info.
        /// </summary>
        /// <param name="repositoryInfo">The repository information.</param>
        /// <param name="noFetch">If set to <c>true</c>, don't fetch anything.</param>
        /// <returns>The git repository.</returns>
        public static GitRepository CreateRepository(RepositoryInfo repositoryInfo, bool noFetch = false)
        {
            bool isDynamicRepository = false;
            string repositoryDirectory = null;

            // TODO: find a better way to check for existing repositories
            if (!string.IsNullOrWhiteSpace(repositoryInfo.Directory))
            {
                repositoryDirectory = repositoryInfo.Directory;
            }
            else
            {
                isDynamicRepository = true;

                var tempRepositoryPath = CalculateTemporaryRepositoryPath(repositoryInfo.Url, repositoryDirectory);
                repositoryDirectory = CreateDynamicRepository(tempRepositoryPath, repositoryInfo.Authentication,
                    repositoryInfo.Url, repositoryInfo.Branch, noFetch);
            }

            // TODO: Should we do something with fetch for existing repositoriess?

            var repository = new Repository(repositoryDirectory);
            return new GitRepository(repository, isDynamicRepository);
        }

        static string CalculateTemporaryRepositoryPath(string targetUrl, string dynamicRepositoryLocation)
        {
            var userTemp = dynamicRepositoryLocation;
            if (string.IsNullOrWhiteSpace(userTemp))
            {
                userTemp = Path.GetTempPath();
            }

            var repositoryName = targetUrl.Split('/', '\\').Last().Replace(".git", string.Empty);
            var possiblePath = Path.Combine(userTemp, repositoryName);

            // Verify that the existing directory is ok for us to use
            if (Directory.Exists(possiblePath))
            {
                if (!GitRepoHasMatchingRemote(possiblePath, targetUrl))
                {
                    var i = 1;
                    var originalPath = possiblePath;
                    bool possiblePathExists;
                    do
                    {
                        possiblePath = string.Concat(originalPath, "_", i++.ToString());
                        possiblePathExists = Directory.Exists(possiblePath);
                    } while (possiblePathExists && !GitRepoHasMatchingRemote(possiblePath, targetUrl));
                }
            }

            return possiblePath;
        }

        static bool GitRepoHasMatchingRemote(string possiblePath, string targetUrl)
        {
            try
            {
                using (var repository = new Repository(possiblePath))
                {
                    return repository.Network.Remotes.Any(r => r.Url == targetUrl);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        static string CreateDynamicRepository(string targetPath, AuthenticationInfo authentication, string repositoryUrl, string targetBranch, bool noFetch)
        {
            if (string.IsNullOrWhiteSpace(targetBranch))
            {
                throw new GitToolsException("Dynamic Git repositories must have a target branch (/b)");
            }

            Log.Info(string.Format("Creating dynamic repository at '{0}'", targetPath));

            var gitDirectory = Path.Combine(targetPath, ".git");
            if (Directory.Exists(targetPath))
            {
                Log.Info("Git repository already exists");
                GitHelper.NormalizeGitDirectory(gitDirectory, authentication, noFetch, targetBranch);

                return gitDirectory;
            }

            CloneRepository(repositoryUrl, gitDirectory, authentication);

            // Normalize (download branches) before using the branch
            GitHelper.NormalizeGitDirectory(gitDirectory, authentication, noFetch, targetBranch);

            return gitDirectory;
        }

        private static void CloneRepository(string repositoryUrl, string gitDirectory, AuthenticationInfo authentication)
        {
            Credentials credentials = null;
            if (!string.IsNullOrWhiteSpace(authentication.Username) && !string.IsNullOrWhiteSpace(authentication.Password))
            {
                Log.Info(string.Format("Setting up credentials using name '{0}'", authentication.Username));

                credentials = new UsernamePasswordCredentials
                {
                    Username = authentication.Username,
                    Password = authentication.Password
                };
            }

            Log.Info(string.Format("Retrieving git info from url '{0}'", repositoryUrl));

            try
            {
                Repository.Clone(repositoryUrl, gitDirectory,
                    new CloneOptions
                    {
                        Checkout = false,
                        CredentialsProvider = (url, usernameFromUrl, types) => credentials
                    });
            }
            catch (LibGit2SharpException ex)
            {
                var message = ex.Message;
                if (message.Contains("401"))
                {
                    throw new GitToolsException("Unauthorised: Incorrect username/password");
                }
                if (message.Contains("403"))
                {
                    throw new GitToolsException("Forbidden: Possbily Incorrect username/password");
                }
                if (message.Contains("404"))
                {
                    throw new GitToolsException("Not found: The repository was not found");
                }

                throw new GitToolsException("There was an unknown problem with the Git repository you provided");
            }
        }
    }
}