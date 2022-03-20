using System.Text.Json.Serialization;

namespace branch_hero.Models
{
    public class DefaultBranchChangeEvent : WebhookEvent
    {
        public DefaultBranchChange Changes { get; set; }
    }

    public class DefaultBranchChange
    {
        [JsonPropertyName("default_branch")]
        public DefaultBranch DefaultBranch { get; set; }
    }

    public class DefaultBranch
    {
        public string From { get; set; }
    }
}
