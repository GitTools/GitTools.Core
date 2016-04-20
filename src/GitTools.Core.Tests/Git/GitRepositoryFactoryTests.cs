namespace GitTools.Tests.Git
{
    using System;
    using System.IO;
    using System.Linq;
    using GitTools.Git;
    using IO;
    using LibGit2Sharp;
    using NUnit.Framework;
    using Shouldly;
    using Testing;

    [TestFixture]
    public class GitRepositoryFactoryTests
    {
        const string DefaultBranchName = "master";
        const string SpecificBranchName = "feature/foo";

        [Test]
        [TestCase(DefaultBranchName, DefaultBranchName)]
        [TestCase(SpecificBranchName, SpecificBranchName)]
        public void WorksCorrectlyWithRemoteRepository(string branchName, string expectedBranchName)
        {
            var repoName = Guid.NewGuid().ToString();
            var tempPath = Path.GetTempPath();
            var tempDir = Path.Combine(tempPath, repoName);
            Directory.CreateDirectory(tempDir);
            string dynamicRepositoryPath = null;

            try
            {
                using (var fixture = new EmptyRepositoryFixture())
                {
                    var expectedDynamicRepoLocation = Path.Combine(tempPath, fixture.RepositoryPath.Split(Path.DirectorySeparatorChar).Last());

                    fixture.Repository.MakeCommits(5);
                    fixture.Repository.CreateFileAndCommit("TestFile.txt");

                    fixture.Repository.CreateBranch(SpecificBranchName);

                    // Copy contents into working directory
                    File.Copy(Path.Combine(fixture.RepositoryPath, "TestFile.txt"), Path.Combine(tempDir, "TestFile.txt"));

                    var repositoryInfo = new RepositoryInfo
                    {
                        Url = fixture.RepositoryPath,
                        Branch = branchName
                    };

                    using (var gitRepository = GitRepositoryFactory.CreateRepository(repositoryInfo))
                    {
                        dynamicRepositoryPath = gitRepository.DotGitDirectory;

                        gitRepository.IsDynamic.ShouldBe(true);
                        gitRepository.DotGitDirectory.ShouldBe(expectedDynamicRepoLocation + Path.DirectorySeparatorChar + ".git");

                        var currentBranch = gitRepository.Repository.Head.CanonicalName;

                        currentBranch.ShouldEndWith(expectedBranchName);
                    }
                }
            }
            finally
            {
                Directory.Delete(tempDir, true);

                if (dynamicRepositoryPath != null)
                {
                    DeleteHelper.DeleteGitRepository(dynamicRepositoryPath);
                }
            }
        }

        [Test]
        public void UpdatesExistingDynamicRepository()
        {
            var repoName = Guid.NewGuid().ToString();
            var tempPath = Path.GetTempPath();
            var tempDir = Path.Combine(tempPath, repoName);
            Directory.CreateDirectory(tempDir);
            string dynamicRepositoryPath = null;

            try
            {
                using (var mainRepositoryFixture = new EmptyRepositoryFixture())
                {
                    mainRepositoryFixture.Repository.MakeCommits(1);

                    var repositoryInfo = new RepositoryInfo
                    {
                        Url = mainRepositoryFixture.RepositoryPath,
                        Branch = "master"
                    };

                    using (var gitRepository = GitRepositoryFactory.CreateRepository(repositoryInfo))
                    {
                        dynamicRepositoryPath = gitRepository.DotGitDirectory;
                    }

                    var newCommit = mainRepositoryFixture.Repository.MakeACommit();

                    using (var gitRepository = GitRepositoryFactory.CreateRepository(repositoryInfo))
                    {
                        mainRepositoryFixture.Repository.DumpGraph();
                        gitRepository.Repository.DumpGraph();
                        gitRepository.Repository.Commits.ShouldContain(c => c.Sha == newCommit.Sha);
                    }
                }
            }
            finally
            {
                Directory.Delete(tempDir, true);

                if (dynamicRepositoryPath != null)
                {
                    DeleteHelper.DeleteGitRepository(dynamicRepositoryPath);
                }
            }
        }

        [Test]
        public void PicksAnotherDirectoryNameWhenDynamicRepoFolderTaken()
        {
            var repoName = Guid.NewGuid().ToString();
            var tempPath = Path.GetTempPath();
            var tempDir = Path.Combine(tempPath, repoName);
            Directory.CreateDirectory(tempDir);
            string expectedDynamicRepoLocation = null;

            try
            {
                using (var fixture = new EmptyRepositoryFixture())
                {
                    fixture.Repository.CreateFileAndCommit("TestFile.txt");
                    File.Copy(Path.Combine(fixture.RepositoryPath, "TestFile.txt"), Path.Combine(tempDir, "TestFile.txt"));
                    expectedDynamicRepoLocation = Path.Combine(tempPath, fixture.RepositoryPath.Split(Path.DirectorySeparatorChar).Last());
                    Directory.CreateDirectory(expectedDynamicRepoLocation);

                    var repositoryInfo = new RepositoryInfo
                    {
                        Url = fixture.RepositoryPath,
                        Branch = "master"
                    };

                    using (var gitRepository = GitRepositoryFactory.CreateRepository(repositoryInfo))
                    {
                        gitRepository.IsDynamic.ShouldBe(true);
                        gitRepository.DotGitDirectory.ShouldBe(expectedDynamicRepoLocation + "_1" + Path.DirectorySeparatorChar + ".git");
                    }
                }
            }
            finally
            {
                Directory.Delete(tempDir, true);
                if (expectedDynamicRepoLocation != null)
                {
                    Directory.Delete(expectedDynamicRepoLocation, true);
                }

                if (expectedDynamicRepoLocation != null)
                {
                    DeleteHelper.DeleteGitRepository(expectedDynamicRepoLocation + "_1");
                }
            }
        }

        [Test]
        public void ThrowsExceptionWhenNotEnoughInfo()
        {
            var tempDir = Path.GetTempPath();

            var repositoryInfo = new RepositoryInfo
            {
                Url = tempDir,
                Branch = "master"
            };

            Should.Throw<Exception>(() => GitRepositoryFactory.CreateRepository(repositoryInfo));
        }

        [Test]
        public void UsingDynamicRepositoryWithFeatureBranchWorks()
        {
            var repoName = Guid.NewGuid().ToString();
            var tempPath = Path.GetTempPath();
            var tempDir = Path.Combine(tempPath, repoName);
            Directory.CreateDirectory(tempDir);

            try
            {
                using (var mainRepositoryFixture = new EmptyRepositoryFixture())
                {
                    mainRepositoryFixture.Repository.MakeACommit();

                    var repositoryInfo = new RepositoryInfo
                    {
                        Url = mainRepositoryFixture.RepositoryPath,
                        Branch = "feature1"
                    };

                    mainRepositoryFixture.Repository.Checkout(mainRepositoryFixture.Repository.CreateBranch("feature1"));

                    Should.NotThrow(() =>
                    {
                        using (var gitRepository = GitRepositoryFactory.CreateRepository(repositoryInfo))
                        {
                            // this code shouldn't throw
                        }
                    });
                }
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Test]
        public void UsingDynamicRepositoryWithoutTargetBranchFails()
        {
            var repoName = Guid.NewGuid().ToString();
            var tempPath = Path.GetTempPath();
            var tempDir = Path.Combine(tempPath, repoName);
            Directory.CreateDirectory(tempDir);

            try
            {
                using (var mainRepositoryFixture = new EmptyRepositoryFixture())
                {
                    mainRepositoryFixture.Repository.MakeACommit();

                    var repositoryInfo = new RepositoryInfo
                    {
                        Url = mainRepositoryFixture.RepositoryPath,
                        Branch = null
                    };

                    Should.Throw<Exception>(() =>
                    {
                        using (var gitRepository = GitRepositoryFactory.CreateRepository(repositoryInfo))
                        {
                            // this code shouldn't throw
                        }
                    });
                }
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Test]
        public void TestErrorThrownForInvalidRepository()
        {
            var repoName = Guid.NewGuid().ToString();
            var tempPath = Path.GetTempPath();
            var tempDir = Path.Combine(tempPath, repoName);
            Directory.CreateDirectory(tempDir);

            try
            {
                var repositoryInfo = new RepositoryInfo
                {
                    Url = "http://127.0.0.1/testrepo.git",
                    Branch = "master"
                };

                Should.Throw<Exception>(() =>
                {
                    using (var gitRepository = GitRepositoryFactory.CreateRepository(repositoryInfo))
                    {
                        // this code shouldn't throw
                    }
                });
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}