using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<DouyinFeedController> _Logger;

        public DouyinFeedController(OperationExecutor operationExecutor, ILogger<DouyinFeedController> logger)
        {
            _OperationExecutor = operationExecutor;
            _Logger = logger;
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
        
        [HttpGet("active-session")]
        [Consumes("application/json")]
        public ValueTask<GetActiveSessionIdByAnchorIdResponse> GetActiveSessionIdByAnchorId(
            [FromQuery] string anchorId,
            [FromServices] GetActiveSessionIdByAnchorIdOperation operation,
            CancellationToken cancellationToken)
            => _OperationExecutor.ExecuteAsync<GetActiveSessionIdByAnchorIdOperation,
                string, GetActiveSessionIdByAnchorIdResponse>(operation, anchorId, cancellationToken);

        [HttpPost("live-feed")]
        [Consumes("application/json")]
        public async Task<IActionResult> AcceptLiveFeed(
            [FromBody] List<DouyinMessage> request,
            [FromServices] HandleLiveFeedOperation operation,
            CancellationToken cancellationToken,
            [FromQuery] string cmd = "",
            [FromQuery] string timestamp = "",
            [FromQuery] string version = "",
            [FromQuery] string push_id = "",
            [FromQuery] string nonce_str = "",
            [FromQuery] string signature = ""
            )
        {
            _Logger.LogInformation($"Received Douyin msg: {request.First().Payload.Content} with version {version}");
            var filteredVersion = Environment.GetEnvironmentVariable("FilteredVersion") ?? "1";

            if (version.Equals(filteredVersion))
            {
                return Ok();
            }

            if (SignatureValidator.ValidateSignature(cmd, timestamp, version, push_id, nonce_str, signature, request))
            {
                await _OperationExecutor.ExecuteAsync<HandleLiveFeedOperation,
                    List<DouyinMessage>, bool>(operation, request, cancellationToken);
                return Ok();
            }

            return Unauthorized("The request is unauthorized");
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
        => _OperationExecutor.ExecuteAsync<ValidateDouyinWebhookOperation,
            DouyinWebhookValidationModel, DouyinWebhookValidateResponse>(operation, input, cancellationToken);
    }
}
