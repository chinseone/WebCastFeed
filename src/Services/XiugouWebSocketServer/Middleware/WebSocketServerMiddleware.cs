using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace XiugouWebSocketServer.Middleware
{
    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _Next;

        private readonly WebSocketServerConnectionManager _Manager;

        private readonly string _DouyinChatPath = "/douyin/chat";

        public WebSocketServerMiddleware(RequestDelegate next,
            WebSocketServerConnectionManager webSocketServerConnectionManager)
        {
            _Next = next;
            _Manager = webSocketServerConnectionManager
                       ?? throw new ArgumentNullException(nameof(webSocketServerConnectionManager));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == _DouyinChatPath && context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("WebSocket Connected");

                var connId = _Manager.AddSocket(webSocket);
                await SendConnIdAsync(webSocket, connId);

                await ReceiveMessage(webSocket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine("Message received");
                        Console.WriteLine($"Message {message}");
                        await RouteJsonMessage(message);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        var id = _Manager.GetAllSockets()
                            .FirstOrDefault(s => s.Value == webSocket).Key;

                        _Manager.GetAllSockets().TryRemove(id, out WebSocket sock);

                        await sock.CloseAsync(
                                    result.CloseStatus.Value,
                                    result.CloseStatusDescription,
                                    CancellationToken.None);

                        Console.WriteLine("Received closed message");
                    }
                });
            }
            else
            {
                Console.WriteLine("Hello from the 2nd request delegate");
                await _Next(context);
            }
        }

        private async Task SendConnIdAsync(WebSocket socket, string connId)
        {
            var buffer = Encoding.UTF8.GetBytes($"ConnID: {connId}");
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task ReceiveMessage(WebSocket webSocket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                    cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }

        public async Task RouteJsonMessage(string message)
        {
            try
            {
                var routeObj = JsonConvert.DeserializeObject<dynamic>(message);
                if (Guid.TryParse(routeObj.To.ToString(), out Guid guidOutput))
                {
                    var toSocket = _Manager
                        .GetAllSockets()[guidOutput.ToString()];

                    if (toSocket != null && toSocket.State == WebSocketState.Open) {
                        await toSocket.SendAsync(Encoding.UTF8.GetBytes(message),
                        WebSocketMessageType.Text, true, CancellationToken.None);
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("No Guid, continue");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Input may not necessary be a JSON");
            }


            Console.WriteLine("Broadcast");
            foreach (var socket in _Manager.GetAllSockets())
            {
                if (socket.Value.State == WebSocketState.Open)
                {
                    await socket.Value.SendAsync(Encoding.UTF8.GetBytes(message),
                        WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
