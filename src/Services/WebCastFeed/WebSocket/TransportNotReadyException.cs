using System;

namespace WebCastFeed.WebSocket
{
    public class TransportNotReadyException : Exception
    {
        public TransportNotReadyException(string message) 
        : base(message){ }
    }
}
