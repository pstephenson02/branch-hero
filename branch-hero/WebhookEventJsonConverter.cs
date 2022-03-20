using branch_hero.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace branch_hero
{
    public class WebhookEventJsonConverter : JsonConverter<WebhookEvent>
    {
        public override WebhookEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Utf8JsonReader readerClone = reader;
            using (var jsonDocument = JsonDocument.ParseValue(ref readerClone))
            {
                if (jsonDocument.RootElement.TryGetProperty("ref", out var refProperty))
                {
                    return JsonSerializer.Deserialize<RefCreatedEvent>(ref reader, options);
                }

                if (jsonDocument.RootElement.TryGetProperty("action", out var actionProperty))
                {
                    switch (actionProperty.GetString())
                    {
                        case "edited":
                            return JsonSerializer.Deserialize<RepositoryEditedEvent>(ref reader, options);
                        default:
                            return JsonSerializer.Deserialize<RepositoryEvent>(ref reader, options);
                    }
                }
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, WebhookEvent webhookEvent, JsonSerializerOptions options)
        {
        }
    }
}
