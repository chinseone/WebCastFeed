using System.Threading;
using System.Threading.Tasks;

namespace WebCastFeed.WebSocket
{
    public interface ITransportFactory
    {
        Task<IAsyncTransport> CreateTransportAsync(CancellationToken cancellationToken);
    }
}
