using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebCastFeed.Models.Requests;
using WebCastFeed.Models.Response;
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
            // var key = Environment.GetEnvironmentVariable("TicketCreationKey") ?? _FakeApiKey;
            // if (string.IsNullOrEmpty(ticketCreationKey) || ticketCreationKey.Equals(_FakeApiKey) || !key.Equals(ticketCreationKey))
            // {
            //     return BadRequest("Invalid TicketCreationKey");
            // }
            var res = _OperationExecutor.ExecuteAsync<CreateTicketOperation,
                CreateTicketRequest, bool>(operation, input, cancellationToken);
            return Ok(res);
        }

        [HttpPost("state")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateTicket(
            [FromBody] UpdateTicketRequest input,
            [FromServices] UpdateTicketOperation operation,
            [FromHeader(Name = "X-Ticket-Update-Key")] string ticketUpdateKey,
            CancellationToken cancellationToken)
        {
            var key = Environment.GetEnvironmentVariable("TicketUpdateKey") ?? _FakeApiKey;
            var checkKey = bool.Parse(Environment.GetEnvironmentVariable("CheckTicketUpdateKey") ?? "false") ;
            if (checkKey && (string.IsNullOrEmpty(ticketUpdateKey) || ticketUpdateKey.Equals(_FakeApiKey) || !key.Equals(ticketUpdateKey)))
            {
                return Unauthorized("Invalid TicketUpdateKey");
            }
            var res = await _OperationExecutor.ExecuteAsync<UpdateTicketOperation,
                UpdateTicketRequest, UpdateTicketResponse>(operation, input, cancellationToken);
            return Ok(res);
        }

        [HttpPost("all")]
        public async Task<IActionResult> GetAllTickets(
            [FromBody] GetAllTicketsRequest request,
            [FromServices] GetAllTicketsOperation operation,
            CancellationToken cancellationToken)
        {
            var res = await _OperationExecutor.ExecuteAsync<GetAllTicketsOperation,
                GetAllTicketsRequest, List<GetTicketByCodeResponse>>(operation, request, cancellationToken);

            if (res == null || res.Count == 0)
            {
                return Unauthorized("Request is unauthorized");
            }

            return Ok(res);
        }

        [HttpPost("launch")]
        public async Task<IActionResult> OnboardOfficialTickets(
            [FromServices] OnboardOfficialTicketsOperation operation,
            CancellationToken cancellationToken)
        {
            var res = await _OperationExecutor.ExecuteAsync<OnboardOfficialTicketsOperation,
                string, bool>(operation, "", cancellationToken);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetTicketByCode(
            [FromQuery]string code,
            [FromServices] GetTicketByCodeOperation operation,
            [FromHeader(Name = "X-Ticket-Get-Key")] string ticketGetKey,
            CancellationToken cancellationToken)
        {
            var key = Environment.GetEnvironmentVariable("TicketGetKey") ?? _FakeApiKey;
            if (string.IsNullOrEmpty(ticketGetKey) || ticketGetKey.Equals(_FakeApiKey) || !key.Equals(ticketGetKey))
            {
                return Unauthorized("Invalid TicketGetKey");
            }

            var res = await _OperationExecutor.ExecuteAsync<GetTicketByCodeOperation,
                string, GetTicketByCodeResponse>(operation, code, cancellationToken);
            return Ok(res);
        }
    }
}
