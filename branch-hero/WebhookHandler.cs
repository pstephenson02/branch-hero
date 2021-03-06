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
                    await CreateGitHubIssueAndNotifyUser(refCreatedEvent.Repository.Id, $"@{refCreatedEvent.Sender.Login}: A new default branch, `{refCreatedEvent.DefaultBranch}`, was created and now protected.");
                }
            }

            if (ev.GetType() == typeof(DefaultBranchChangeEvent))
            {
                // If the new default branch already has protection, just leave it
                try
                {
                    await githubClient.Repository.Branch.GetBranchProtection(ev.Repository.Id, ev.Repository.DefaultBranch);
                }
                catch (Octokit.NotFoundException e)
                {
                    if ("Branch not protected".Equals(e.Message))
                    {
                        await ProtectDefaultBranch(ev.Repository.Id, ev.Repository.DefaultBranch);
                        await CreateGitHubIssueAndNotifyUser(ev.Repository.Id, $"@{ev.Sender.Login}: The default branch was changed to `{ev.Repository.DefaultBranch}` and is now protected.");
                    }
                    else
                    {
                        throw e;
                    }
                }
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

        private async Task CreateGitHubIssueAndNotifyUser(long repositoryId, string message)
        {
            var newIssue = new NewIssue("branch-hero: New branch protection added.")
            {
                Body = message
            };
            await githubClient.Issue.Create(repositoryId, newIssue);
        }
    }
}
