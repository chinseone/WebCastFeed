using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebCastFeed.Models.Requests;
using WebCastFeed.Operations;

namespace WebCastFeed.Controllers
{
    [Route("v1/h5")]
    [ApiController]
    public class H5Controller : Controller
    {
        private readonly OperationExecutor _OperationExecutor;

        public H5Controller(OperationExecutor operationExecutor)
        {
            _OperationExecutor = operationExecutor;
        }

        [HttpPost("profile")]
        [Consumes("application/json")]
        public ValueTask<bool> CreateH5Profile(
            [FromBody] CreateH5ProfileRequest request,
            [FromServices] CreateH5ProfileOperation operation,
            CancellationToken cancellationToken)
            => _OperationExecutor.ExecuteAsync<CreateH5ProfileOperation,
                CreateH5ProfileRequest, bool>(operation, request, cancellationToken);
    }
}
