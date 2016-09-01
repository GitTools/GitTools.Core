namespace GitTools.Git
{
    using System;
    using System.IO;
    using System.Linq;
    using LibGit2Sharp;
    using Logging;

    public static class DynamicRepositories
    {
        static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Creates a dynamic repository based on the repository info
        /// </summary>
        /// <param name="repositoryInfo">The source repository information.</param>
        /// <param name="dynamicRepsitoryPath">The path to create the dynamic repository, NOT thread safe.</param>
        /// <param name="targetBranch"></param>
        /// <param name="targetCommit"></param>
        /// <param name="noFetch">If set to <c>true</c>, don't fetch anything.</param>
        /// <returns>The git repository.</returns>
        public static Repository CreateOrOpen(RepositoryInfo repositoryInfo, string dynamicRepsitoryPath, string targetBranch, string targetCommit, bool noFetch = false)
        {
            if (string.IsNullOrWhiteSpace(targetBranch))
                throw new GitToolsException("Dynamic Git repositories must have a target branch");
            if (string.IsNullOrWhiteSpace(targetCommit))
                throw new GitToolsException("Dynamic Git repositories must have a target commit");

            var tempRepositoryPath = CalculateTemporaryRepositoryPath(repositoryInfo.Url, dynamicRepsitoryPath);
            var dynamicRepositoryPath = CreateDynamicRepository(tempRepositoryPath, repositoryInfo, noFetch, targetBranch, targetCommit);

            return new Repository(dynamicRepositoryPath);
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

        static string CreateDynamicRepository(string targetPath, RepositoryInfo repositoryInfo, bool noFetch, string targetBranch, string targetCommit)
        {
            Log.Info(string.Format("Creating dynamic repository at '{0}'", targetPath));

            var gitDirectory = Path.Combine(targetPath, ".git");
            if (Directory.Exists(targetPath))
            {
                Log.Info("Git repository already exists");
                CheckoutCommit(targetCommit, gitDirectory);
                GitRepositoryHelper.NormalizeGitDirectory(gitDirectory, repositoryInfo.Authentication, noFetch, targetBranch);

                return gitDirectory;
            }

            CloneRepository(repositoryInfo.Url, gitDirectory, repositoryInfo.Authentication);
            CheckoutCommit(targetCommit, gitDirectory);

            // Normalize (download branches) before using the branch
            GitRepositoryHelper.NormalizeGitDirectory(gitDirectory, repositoryInfo.Authentication, noFetch, targetBranch);

            return gitDirectory;
        }

        static void CheckoutCommit(string targetCommit, string gitDirectory)
        {
            using (var repo = new Repository(gitDirectory))
            {
                Log.Info(string.Format("Checking out {0}", targetCommit));
                repo.Checkout(targetCommit);
            }
        }

        static void CloneRepository(string repositoryUrl, string gitDirectory, AuthenticationInfo authentication)
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