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

        public TicketController(OperationExecutor operationExecutor)
        {
            _OperationExecutor = operationExecutor ?? throw new ArgumentNullException(nameof(operationExecutor));
        }

        [HttpPost("")]
        [Consumes("application/json")]
        public ValueTask<bool> CreateTicket(
            [FromBody]CreateTicketRequest input,
            [FromServices] CreateTicketOperation operation,
            CancellationToken cancellationToken)
        => _OperationExecutor.ExecuteAsync<CreateTicketOperation,
            CreateTicketRequest, bool>(operation, input, cancellationToken);
    }
}
