using System.Text.Json.Serialization;

namespace branch_hero.Models
{
    public class Repository
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("default_branch")]
        public string DefaultBranch { get; set; }
    }
}
