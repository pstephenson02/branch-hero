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
            if (ev.GetType() == typeof(RefCreatedEvent))
            {
                RefCreatedEvent refCreatedEvent = (RefCreatedEvent)ev;
                if (refCreatedEvent.IsFirstBranchInRepository())
                {
                    await ProtectDefaultBranch(refCreatedEvent.Repository.Id, refCreatedEvent.DefaultBranch);
                }
            }

            if (ev.GetType() == typeof(DefaultBranchChangeEvent))
            {
                // todo: Check if new default branch already has branch protection
                await ProtectDefaultBranch(ev.Repository.Id, ev.Repository.DefaultBranch);
            }

            return true;
        }

        private async Task<BranchProtectionSettings> ProtectDefaultBranch(long repositoryId, string defaultBranchName)
        {
            var branchProtectionSettings = new BranchProtectionRequiredReviewsUpdate(
                true, // Dismiss stale reviews on new commits
                false, // Don't require codeowners reviews
                1 // Required number of approvers
            );
            var bpRequest = new BranchProtectionSettingsUpdate(branchProtectionSettings);

            return await githubClient.Repository.Branch.UpdateBranchProtection(repositoryId, defaultBranchName, bpRequest);
        }
    }
}
