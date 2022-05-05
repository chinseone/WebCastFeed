using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;

namespace WebCastFeed.Operations
{
    public class CreateH5ProfileOperation : IAsyncOperation<CreateH5ProfileRequest, bool>
    {
        public async ValueTask<bool> ExecuteAsync(CreateH5ProfileRequest input, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
