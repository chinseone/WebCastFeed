using Microsoft.AspNetCore.Mvc;
using WebCastFeed.Models.Requests;

namespace WebCastFeed.Controllers
{
    [Route("v1/blue-dash")]
    [ApiController]
    public class ChatController : Controller
    {
        [HttpPost("messages")]
        [Consumes("application/json")]
        public IActionResult AcceptMessages(
            [FromBody]MessageBody message)
        {
            return Ok();
        }
    }
}
