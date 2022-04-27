using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;

namespace WebCastFeed.Operations
{
    public class HandleLiveFeedOperation : IAsyncOperation<List<DouyinMessage>, bool>
    {
        public async ValueTask<bool> ExecuteAsync(List<DouyinMessage> input, CancellationToken cancellationToken = default)
        {
            return true;
        }
    }
}
