using System.Text.Json.Serialization;

namespace branch_hero.Models
{
    [JsonConverter(typeof(WebhookEventJsonConverter))]
    public abstract class WebhookEvent
    {
    }
}
