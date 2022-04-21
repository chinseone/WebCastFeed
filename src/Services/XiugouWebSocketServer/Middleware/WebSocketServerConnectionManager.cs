using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace XiugouWebSocketServer.Middleware
{
    public class WebSocketServerConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _Sockets = new ConcurrentDictionary<string, WebSocket>();

        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _Sockets;
        }

        public string AddSocket(WebSocket socket)
        {
            var connId = Guid.NewGuid().ToString();

            _Sockets.TryAdd(connId, socket);
            Console.WriteLine($"Connection Added: {connId}");

            return connId;
        }
    }
}
