using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models;
using WebCastFeed.Models.Requests;
using WebCastFeed.Models.Response;
using WebCastFeed.Operations;

namespace WebCastFeed.Controllers
{
    [Route("v1/douyin")]
    [ApiController]
    public class DouyinFeedController : Controller
    {
        private readonly OperationExecutor _OperationExecutor;

        public DouyinFeedController(OperationExecutor operationExecutor)
        {
            _OperationExecutor = operationExecutor;
        }

        [HttpPost("start-game")]
        [Consumes("application/json")]
        public ValueTask<DouyinStartGameResponse> StartGame(
            [FromBody] DouyinStartGameRequest request,
            [FromServices] DouyinStartGameOperation operation,
            CancellationToken cancellationToken)
        => _OperationExecutor.ExecuteAsync<DouyinStartGameOperation,
                DouyinStartGameRequest, DouyinStartGameResponse>(operation, request, cancellationToken);

        [HttpPost("stop-game")]
        [Consumes("application/json")]
        public ValueTask<DouyinStopGameResponse> StopGame(
            [FromBody] DouyinStopGameRequest request,
            [FromServices] DouyinStopGameOperation operation,
            CancellationToken cancellationToken)
            => _OperationExecutor.ExecuteAsync<DouyinStopGameOperation,
                DouyinStopGameRequest, DouyinStopGameResponse>(operation, request, cancellationToken);


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
        => _OperationExecutor.ExecuteAsync<ValidateDouyinWebhookOperation,
            DouyinWebhookValidationModel, DouyinWebhookValidateResponse>(operation, input, cancellationToken);
    }
}
