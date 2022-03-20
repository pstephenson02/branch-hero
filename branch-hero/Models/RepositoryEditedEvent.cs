using System.Text.Json.Serialization;

namespace branch_hero.Models
{
    public class RepositoryEditedEvent : RepositoryEvent
    {
        public Changes Changes { get; set; }
    }

    public class Changes
    {
        [JsonPropertyName("default_branch")]
        public DefaultBranch DefaultBranch { get; set; }
    }

    public class DefaultBranch
    {
        public string From { get; set; }
    }
}
