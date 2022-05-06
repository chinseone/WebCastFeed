using System;

namespace WebCastFeed.WebSocket
{
    public class TransportClosedEventArgs : EventArgs
    {
        public TransportClosedEventArgs() { }

        public TransportClosedEventArgs(Exception e)
        {
            Exception = e;
        }

        public Exception Exception { get; }
    }
}
