using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebCastFeed.Models.Requests;
using WebCastFeed.Operations;

namespace WebCastFeed.Controllers
{
    [Route("v1/ticket")]
    [ApiController]
    public class TicketController : Controller
    {
        private readonly OperationExecutor _OperationExecutor;
        private const string _FakeApiKey = "fake-api-key";

        public TicketController(OperationExecutor operationExecutor)
        {
            _OperationExecutor = operationExecutor ?? throw new ArgumentNullException(nameof(operationExecutor));
        }

        [HttpPost("")]
        [Consumes("application/json")]
        public IActionResult CreateTicket(
            [FromBody]CreateTicketRequest input,
            [FromServices] CreateTicketOperation operation,
            [FromHeader(Name="X-Ticket-Creation-Key")]string ticketCreationKey,
            CancellationToken cancellationToken)
        {
            var key = Environment.GetEnvironmentVariable("TicketCreationKey") ?? _FakeApiKey;
            if (string.IsNullOrEmpty(ticketCreationKey) || ticketCreationKey.Equals(_FakeApiKey) || !key.Equals(ticketCreationKey))
            {
                return BadRequest("Invalid TicketCreationKey");
            }
            var res = _OperationExecutor.ExecuteAsync<CreateTicketOperation,
                CreateTicketRequest, bool>(operation, input, cancellationToken);
            return Ok(res);
        }
    }
}
