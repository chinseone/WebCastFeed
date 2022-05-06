using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;

namespace WebCastFeed.WebSocket
{
    public class WebSocketClient : IWebSocketClient
    {
        private IAsyncTransport _RootTransport;

        private ITransportFactory _TransportFactory;

        public WebSocketClient(ITransportFactory transportFactory)
        {
            _TransportFactory = transportFactory ?? throw new ArgumentNullException(nameof(transportFactory));
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            await CreateNewTransportAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task SendAsync(List<DouyinMessage> request, IAsyncTransport transport)
        {
            await transport.SendAsync(request, CancellationToken.None).ConfigureAwait(false);
        }

        private void OnTransportClosed(object sender, TransportClosedEventArgs args)
        {
            // TODO
        }

        private void OnTransportMessage(object sender, MessageEventArgs args)
        {
            // TODO
            Console.WriteLine("Client received message");
        }

        public async Task<IAsyncTransport> CreateNewTransportAsync(CancellationToken cancellationToken)
        {
            var transport = await _TransportFactory.CreateTransportAsync(cancellationToken).ConfigureAwait(false);
            transport.OnClosed += OnTransportClosed;
            transport.OnMessage += OnTransportMessage;

            if (_RootTransport == null)
            {
                _RootTransport = transport;
            }
            return transport;
        }
    }
}
