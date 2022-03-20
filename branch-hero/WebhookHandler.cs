using branch_hero.Models;
using Microsoft.Extensions.Options;
using Octokit;
using System;
using System.Threading.Tasks;

namespace branch_hero
{
    public interface IWebhookHandler
    {
        public Task<bool> Handle(WebhookEvent ev);
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

        public async Task<bool> Handle(WebhookEvent ev)
        {
            if (ev.GetType() == typeof(RefCreatedEvent) || ev.GetType() == typeof(DefaultBranchChangeEvent))
            {
                var branchProtectionSettings = new BranchProtectionRequiredReviewsUpdate(true, false, 1);
                var bpRequest = new BranchProtectionSettingsUpdate(branchProtectionSettings);

                await githubClient.Repository.Branch.UpdateBranchProtection(ev.Repository.Id, ev.Repository.DefaultBranch, bpRequest);
            }

            return true;
        }
    }
}
