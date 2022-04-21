﻿using System;
using System.Threading;
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
            var key = Environment.GetEnvironmentVariable("TicketCreationKey") ?? _FakeApiKey;
            if (string.IsNullOrEmpty(ticketCreationKey) || ticketCreationKey.Equals(_FakeApiKey) || !key.Equals(ticketCreationKey))
            {
                return BadRequest("Invalid TicketCreationKey");
            }
            var res = _OperationExecutor.ExecuteAsync<CreateTicketOperation,
                CreateTicketRequest, bool>(operation, input, cancellationToken);
            return Ok(res);
        }

        [HttpPost("state")]
        [Consumes("application/json")]
        public IActionResult UpdateTicket(
            [FromBody] UpdateTicketRequest input,
            [FromServices] UpdateTicketOperation operation,
            [FromHeader(Name = "X-Ticket-Update-Key")] string ticketUpdateKey,
            CancellationToken cancellationToken)
        {
            var key = Environment.GetEnvironmentVariable("TicketUpdateKey") ?? _FakeApiKey;
            if (string.IsNullOrEmpty(ticketUpdateKey) || ticketUpdateKey.Equals(_FakeApiKey) || !key.Equals(ticketUpdateKey))
            {
                return Unauthorized("Invalid TicketUpdateKey");
            }
            var res = _OperationExecutor.ExecuteAsync<UpdateTicketOperation,
                UpdateTicketRequest, bool>(operation, input, cancellationToken);
            return Ok(res);
        }

        [HttpGet]
        public IActionResult GetTicketByCode(
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

            var res = _OperationExecutor.ExecuteAsync<GetTicketByCodeOperation,
                string, GetTicketByCodeResponse>(operation, code, cancellationToken);
            return Ok(res);
        }
    }
}