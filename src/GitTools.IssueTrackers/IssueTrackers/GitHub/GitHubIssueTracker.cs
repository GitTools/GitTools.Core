namespace GitTools.IssueTrackers.GitHub
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Logging;
    using Octokit;
    using Issue = IssueTrackers.Issue;

    public class GitHubIssueTracker : IssueTrackerBase
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly Dictionary<string, User> _userCache = new Dictionary<string, User>();

        public GitHubIssueTracker(IIssueTrackerContext issueTrackerContext)
            : base(issueTrackerContext)
        {
            
        }

        public override async Task<IEnumerable<Issue>> GetIssuesAsync(IssueTrackerFilter filter)
        {
            var gitHubClient = new GitHubClient(new ProductHeaderValue("GitReleaseNotes"));

            var authentication = IssueTrackerContext.Authentication;
            if (authentication != null)
            {
                if (authentication.IsTokenAuthentication())
                {
                    gitHubClient.Credentials = new Octokit.Credentials(authentication.Token);
                }

                if (authentication.IsUsernameAndPasswordAuthentication())
                {
                    gitHubClient.Credentials = new Octokit.Credentials(authentication.Username, authentication.Password);
                }
            }

            string organisation;
            string repository;
            GetRepository(out organisation, out repository);

            var repositoryIssueRequest = PrepareFilter(filter);
            var forRepository = await gitHubClient.Issue.GetAllForRepository(organisation, repository, repositoryIssueRequest);

            var readOnlyList = forRepository.Where(i => i.ClosedAt > filter.Since);

            return readOnlyList.Select(i => new Issue("#" + i.Number.ToString(CultureInfo.InvariantCulture))
            {
                DateClosed = i.ClosedAt,
                Url = i.HtmlUrl.ToString(),
                Title = i.Title,
                IssueType = i.PullRequest == null ? IssueType.Issue : IssueType.PullRequest,
                Labels = i.Labels.Select(l => l.Name).ToArray(),
                Contributors = i.PullRequest == null ? new GitTools.IssueTrackers.Contributor[0] : new []
                {
                    new GitTools.IssueTrackers.Contributor(GetUserName(gitHubClient, i.User), i.User.Login, i.User.HtmlUrl)
                }
            });
        }

        private string GetUserName(GitHubClient client, User u)
        {
            var login = u.Login;
            if (!_userCache.ContainsKey(login))
            {
                _userCache.Add(login, string.IsNullOrEmpty(u.Name) ? client.User.Get(login).Result : u);
            }

            var user = _userCache[login];
            if (user != null)
            {
                return user.Name;
            }

            return null;
        }

        private RepositoryIssueRequest PrepareFilter(IssueTrackerFilter filter)
        {
            var repositoryIssueRequest = new RepositoryIssueRequest
            {
                Filter = IssueFilter.All,
                Since = filter.Since,
            };

            if (filter.IncludeOpen && filter.IncludeClosed)
            {
                repositoryIssueRequest.State = ItemState.All;
            }
            else if (filter.IncludeOpen)
            {
                repositoryIssueRequest.State = ItemState.Open;
            }
            else if (filter.IncludeClosed)
            {
                repositoryIssueRequest.State = ItemState.Closed;
            }

            return repositoryIssueRequest;
        }

        private void GetRepository(out string organisation, out string repository)
        {
            //if (RemotePresentWhichMatches)
            //{
            //    if (TryRemote(out organisation, out repository, "upstream"))
            //    {
            //        return;
            //    }

            //    if (TryRemote(out organisation, out repository, "origin"))
            //    {
            //        return;
            //    }

            //    var remoteName = _gitRepository.Network.Remotes.First(r => r.Url.ToLower().Contains("github.com")).Name;
            //    if (TryRemote(out organisation, out repository, remoteName))
            //    {
            //        return;
            //    }
            //}

            var repoParts = IssueTrackerContext.ProjectId.Split('/');
            organisation = repoParts[0];
            repository = repoParts[1];
        }

        //private bool TryRemote(out string organisation, out string repository, string remoteName)
        //{
        //    var remote = _gitRepository.Network.Remotes[remoteName];
        //    if (remote != null && remote.Url.ToLower().Contains("github.com"))
        //    {
        //        var urlWithoutGitExtension = remote.Url.EndsWith(".git") ? remote.Url.Substring(0, remote.Url.Length - 4) : remote.Url;
        //        var match = Regex.Match(urlWithoutGitExtension, "github.com[/:](?<org>.*?)/(?<repo>.*)");
        //        if (match.Success)
        //        {
        //            organisation = match.Groups["org"].Value;
        //            repository = match.Groups["repo"].Value;
        //            return true;
        //        }
        //    }

        //    organisation = null;
        //    repository = null;
        //    return false;
        //}
    }
}