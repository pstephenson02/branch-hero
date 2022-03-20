using System.Text.Json.Serialization;

namespace branch_hero.Models
{
    public class RefCreatedEvent : WebhookEvent
    {
        public string Ref { get; set; }
        [JsonPropertyName("ref_type")]
        public string RefType { get; set; }
        [JsonPropertyName("master_branch")]
        public string DefaultBranch { get; set; }

        public bool IsFirstBranchInRepository() => Ref.Equals(DefaultBranch);
    }
}
