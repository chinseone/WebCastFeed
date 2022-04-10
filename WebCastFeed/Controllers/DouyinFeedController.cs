using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models;
using WebCastFeed.Operations;

namespace WebCastFeed.Controllers
{
    [Route("v1/douyin")]
    [ApiController]
    public class DouyinFeedController : Controller
    {
        private readonly OperationExecutor _operationExecutor;

        public DouyinFeedController(OperationExecutor operationExecutor)
        {
            _operationExecutor = operationExecutor;
        }

        [HttpGet("health")]
        public string Health()
        {
            return "healthy";
        }

        [HttpPost("webhook-validation")]
        [Consumes("application/json")]
        public ValueTask<string> ValidateDouyinWebhook(
            [FromBody]DouyinWebhookValidationModel input,
            [FromServices] ValidateDouyinWebhookOperation operation,
            CancellationToken cancellationToken)
        => _operationExecutor.ExecuteAsync<ValidateDouyinWebhookOperation,
            DouyinWebhookValidationModel, string>(operation, input, cancellationToken);
    }
}
