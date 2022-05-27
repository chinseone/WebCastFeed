using Microsoft.AspNetCore.Mvc;
using WebCastFeed.Models.Requests;
using XiugouChatHub;
using XiugouChatHub.Operations;

namespace WebCastFeed.Controllers
{
    [Route("v1/blue-dash")]
    [ApiController]
    public class ChatController : Controller
    {
        private readonly OperationExecutor _OperationExecutor;

        public ChatController(OperationExecutor operationExecutor)
        {
            _OperationExecutor = operationExecutor;
        }

        [HttpPost("messages")]
        [Consumes("application/json")]
        public async Task<IActionResult> AcceptMessages(
            [FromBody]MessageBody message,
            [FromServices] StoreChatInformationOperation operation,
            CancellationToken cancellationToken)
        {
            await _OperationExecutor.ExecuteAsync<StoreChatInformationOperation,
                MessageBody, bool>(operation, message, cancellationToken);
            return Ok();
        }
    }
}
