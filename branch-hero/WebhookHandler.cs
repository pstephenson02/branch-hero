using branch_hero.Models;
using Microsoft.Extensions.Options;
using Octokit;
using System;
using System.Threading.Tasks;

namespace branch_hero
{
    public interface IWebhookHandler
    {
        public Task<bool> Handle(RepositoryEvent ev);
    }

    public class WebhookHandler : IWebhookHandler
    {
        private readonly GitHubClient githubClient;

        public WebhookHandler(IOptions<GitHubSettings> githubSettings)
        {
            githubClient = new GitHubClient(new ProductHeaderValue("branch-hero"), new Uri(githubSettings.Value.Url))
            {
                Credentials = new Credentials(githubSettings.Value.Token)
            };
        }

        public async Task<bool> Handle(RepositoryEvent ev)
        {
            var repos = await githubClient.Repository.Get(ev.Repository.Id);
            return true;
        }
    }
}
