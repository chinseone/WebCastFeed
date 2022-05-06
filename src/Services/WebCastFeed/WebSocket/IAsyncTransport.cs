using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;

namespace WebCastFeed.WebSocket
{
    public interface IAsyncTransport
    {
        event EventHandler OnReady;

        event EventHandler<TransportClosedEventArgs> OnClosed;

        event EventHandler<MessageEventArgs> OnMessage;

        event EventHandler<TransportReconnectEventArgs> OnReconnect;

        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);

        Task SendAsync(List<DouyinMessage> douyinMessages, CancellationToken cancellationToken);
    }
}
