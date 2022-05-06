using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;

namespace WebCastFeed.WebSocket
{
    public interface IWebSocketClient
    {
        Task InitializeAsync(CancellationToken cancellationToken);

        Task SendAsync(List<DouyinMessage> request, IAsyncTransport transport);
    }
}
