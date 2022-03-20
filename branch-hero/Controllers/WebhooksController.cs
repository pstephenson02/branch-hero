using branch_hero.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace branch_hero.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhooksController : ControllerBase
    {
        private readonly IWebhookHandler _webhookHandler;

        public WebhooksController(IWebhookHandler webhookHandler)
        {
            _webhookHandler = webhookHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Post(WebhookEvent ev)
        {
            await _webhookHandler.Handle(ev);
            return Ok();
        }
    }
}
