using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebCastFeed.WebSocket
{
    public class WebSocketTransportFactory : ITransportFactory
    {
        private readonly string _WebSocketUri;
        private readonly int _Port;
        private readonly int _MaxRetries;

        private readonly EventHandler<TransportClosedEventArgs> _TransportClosedHandler;

        public WebSocketTransportFactory(string websocketHost, int websocketPort, int maxRetries, EventHandler<TransportClosedEventArgs> transportClosedHandler)
        {
            _WebSocketUri = !string.IsNullOrWhiteSpace(websocketHost) ? websocketHost : throw new ArgumentException("Websocket host can't be null or empty.", nameof(websocketHost));
            _Port = websocketPort > 0 ? websocketPort : throw new ArgumentException("Websocket port must be a positive integer.", nameof(websocketPort));
            _MaxRetries = maxRetries > 0 ? maxRetries : throw new ArgumentException("Max retries must be a positive integer.", nameof(maxRetries));
            _TransportClosedHandler = transportClosedHandler ?? throw new ArgumentNullException(nameof(transportClosedHandler));
        }

        public async Task<IAsyncTransport> CreateTransportAsync(CancellationToken cancellationToken)
        {
            var websocketTransport = new WebSocketTransport(_WebSocketUri, _Port, _MaxRetries);
            websocketTransport.OnClosed += _TransportClosedHandler;
            await websocketTransport.StartAsync(cancellationToken).ConfigureAwait(false);
            return websocketTransport;
        }
    }
}
