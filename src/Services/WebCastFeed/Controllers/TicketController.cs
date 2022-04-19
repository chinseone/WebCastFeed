using System;
using Microsoft.AspNetCore.Mvc;

namespace WebCastFeed.Controllers
{
    [Route("v1/ticket")]
    [ApiController]
    public class TicketController : Controller
    {
        private readonly OperationExecutor _operationExecutor;

        public TicketController(OperationExecutor operationExecutor)
        {
            _operationExecutor = operationExecutor ?? throw new ArgumentNullException(nameof(operationExecutor));
        }
    }
}
