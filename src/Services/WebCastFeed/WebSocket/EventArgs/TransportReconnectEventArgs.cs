using System;

namespace WebCastFeed.WebSocket
{
    public class TransportReconnectEventArgs : EventArgs
    {
        public TransportReconnectEventArgs() { }

        public bool IsFailed { get; set; }
    }
}
