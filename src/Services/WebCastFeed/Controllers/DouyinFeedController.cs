using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models;
using WebCastFeed.Models.Response;
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
        public ValueTask<DouyinWebhookValidateResponse> ValidateDouyinWebhook(
            [FromBody]DouyinWebhookValidationModel input,
            [FromServices] ValidateDouyinWebhookOperation operation,
            CancellationToken cancellationToken)
        => _operationExecutor.ExecuteAsync<ValidateDouyinWebhookOperation,
            DouyinWebhookValidationModel, DouyinWebhookValidateResponse>(operation, input, cancellationToken);
    }
}
