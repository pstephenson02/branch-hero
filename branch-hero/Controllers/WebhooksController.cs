using branch_hero.Models;
using Microsoft.AspNetCore.Mvc;

namespace branch_hero.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhooksController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post(RepositoryEvent ev)
        {
            return Ok();
        }
    }
}
