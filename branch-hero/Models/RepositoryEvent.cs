namespace branch_hero.Models
{
    public class RepositoryEvent : WebhookEvent
    {
        public string Action { get; set; }
    }
}
