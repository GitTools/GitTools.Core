namespace GitTools.Tests
{
    using GitTools.Git;
    using LibGit2Sharp;
    using NUnit.Framework;
    using Shouldly;
    using Testing;

    public class GitHelperTests
    {
        [Test]
        public void NormalisationOfPullRequestsWithFetch()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.Repository.MakeACommit();

                fixture.Repository.Checkout(fixture.Repository.CreateBranch("feature/foo"));
                fixture.Repository.MakeACommit();
                var commit = fixture.Repository.CreatePullRequestRef("feature/foo", "master", prNumber: 3);
                using (var localFixture = fixture.CloneRepository())
                {
                    localFixture.Checkout(commit.Sha);
                    GitRepositoryHelper.NormalizeGitDirectory(localFixture.RepositoryPath, new AuthenticationInfo(), noFetch: false, currentBranch: string.Empty);

                    var normalisedPullBranch = localFixture.Repository.Branches["pull/3/merge"];
                    normalisedPullBranch.ShouldNotBe(null);
                }
            }
        }

        [Test]
        public void NormalisationOfTag()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.Repository.MakeACommit();

                fixture.Repository.Checkout(fixture.Repository.CreateBranch("feature/foo"));
                fixture.Repository.MakeACommit();

                fixture.BranchTo("release/2.0.0");
                fixture.MakeACommit();
                fixture.MakeATaggedCommit("2.0.0-rc.1");
                fixture.Checkout("master");
                fixture.MergeNoFF("release/2.0.0");
                fixture.Repository.Branches.Remove(fixture.Repository.Branches["release/2.0.0"]);
                var remoteTagSha = fixture.Repository.Tags["2.0.0-rc.1"].Target.Sha;

                using (var localFixture = fixture.CloneRepository())
                {
                    localFixture.Checkout(remoteTagSha);
                    GitRepositoryHelper.NormalizeGitDirectory(localFixture.RepositoryPath, new AuthenticationInfo(), noFetch: false, currentBranch: string.Empty);

                    localFixture.Repository.Head.FriendlyName.ShouldBe("(no branch)");
                    localFixture.Repository.Head.Tip.Sha.ShouldBe(remoteTagSha);
                }
            }
        }

        [Test]
        public void NormalisationOfPullRequestsWithoutFetch()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.Repository.MakeACommit();

                fixture.Repository.Checkout(fixture.Repository.CreateBranch("feature/foo"));
                fixture.Repository.MakeACommit();
                var commit = fixture.Repository.CreatePullRequestRef("feature/foo", "master", prNumber: 3, allowFastFowardMerge: true);
                using (var localFixture = fixture.CloneRepository())
                {
                    localFixture.Checkout(commit.Sha);
                    GitRepositoryHelper.NormalizeGitDirectory(localFixture.RepositoryPath, new AuthenticationInfo(), noFetch: true, currentBranch: "refs/pull/3/merge");

                    var normalisedPullBranch = localFixture.Repository.Branches["pull/3/merge"];
                    normalisedPullBranch.ShouldNotBe(null);
                }
            }
        }

        [Test]
        public void UpdatesLocalBranchesWhen()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.Repository.MakeACommit();

                fixture.Repository.Checkout(fixture.Repository.CreateBranch("feature/foo"));
                fixture.Repository.MakeACommit();
                using (var localFixture = fixture.CloneRepository())
                {
                    localFixture.Checkout("feature/foo");
                    // Advance remote
                    var advancedCommit = fixture.Repository.MakeACommit();
                    GitRepositoryHelper.NormalizeGitDirectory(localFixture.RepositoryPath, new AuthenticationInfo(), noFetch: false, currentBranch: null);

                    var normalisedBranch = localFixture.Repository.Branches["feature/foo"];
                    normalisedBranch.ShouldNotBe(null);
                    normalisedBranch.Tip.Sha.ShouldBe(advancedCommit.Sha);
                }
            }
        }

        [Test]
        public void UpdatesCurrentBranch()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.Repository.MakeACommit();
                fixture.Repository.Checkout(fixture.Repository.CreateBranch("develop"));
                fixture.Repository.MakeACommit();
                fixture.Repository.Checkout("master");
                using (var localFixture = fixture.CloneRepository())
                {
                    // Advance remote
                    fixture.Repository.Checkout("develop");
                    var advancedCommit = fixture.Repository.MakeACommit();
                    Commands.Fetch((Repository)localFixture.Repository, localFixture.Repository.Network.Remotes["origin"].Name, new string[0], null, null);
                    localFixture.Repository.Checkout(advancedCommit.Sha);
                    GitRepositoryHelper.NormalizeGitDirectory(localFixture.RepositoryPath, new AuthenticationInfo(), noFetch: false, currentBranch: "ref/heads/develop");

                    var normalisedBranch = localFixture.Repository.Branches["develop"];
                    normalisedBranch.ShouldNotBe(null);
                    normalisedBranch.Tip.Sha.ShouldBe(advancedCommit.Sha);
                    localFixture.Repository.Head.Tip.Sha.ShouldBe(advancedCommit.Sha);
                }
            }
        }

        [Test]
        public void ShouldNotChangeBranchWhenNormalizingTheDirectory()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.Repository.MakeATaggedCommit("v1.0.0");

                fixture.Repository.Checkout(fixture.Repository.CreateBranch("develop"));
                var lastCommitOnDevelop = fixture.Repository.MakeACommit();

                fixture.Repository.Checkout(fixture.Repository.CreateBranch("feature/foo"));
                fixture.Repository.MakeACommit();

                using (var localFixture = fixture.CloneRepository())
                {
                    localFixture.Repository.Checkout("origin/develop");

                    // Another commit on feature/foo will force an update
                    fixture.Checkout("feature/foo");
                    fixture.Repository.MakeACommit();

                    GitRepositoryHelper.NormalizeGitDirectory(localFixture.RepositoryPath, new AuthenticationInfo(), noFetch: false, currentBranch: null);
                    
                    localFixture.Repository.Head.Tip.Sha.ShouldBe(lastCommitOnDevelop.Sha);
                }
            }
        }
    }
}