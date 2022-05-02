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
            Console.WriteLine($"Path: {context.Request.Path}");
            Console.WriteLine($"Websocket request: {context.WebSockets.IsWebSocketRequest}");

            var headers = context.Request.Headers;
            foreach (var header in headers)
            {
                Console.WriteLine($"Header {header.Key} : {header.Value}");
            }
            
            if (context.Request.Path == _DouyinChatPath && context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("WebSocket Connected");

                var connId = _Manager.AddSocket(webSocket);
                // await SendConnIdAsync(webSocket, connId);

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

                        if (sock.State == WebSocketState.Open)
                        {
                            try
                            {
                                await sock.CloseAsync(
                                    result.CloseStatus.Value,
                                    result.CloseStatusDescription,
                                    CancellationToken.None);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Close socket threw exception");
                            }
                        }
                        
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
            var allSockets = _Manager.GetAllSockets();
            Console.WriteLine($"Broadcasting to all {allSockets.Count} listeners...");
            foreach (var socket in allSockets)
            {
                while (socket.Value.State == WebSocketState.Open)
                {
                    var messageToSend = Utf8ToGB2312(message);
                    try
                    {
                        await socket.Value.SendAsync(Encoding.UTF8.GetBytes(messageToSend),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception while sending json to all sockets");
                        break;
                    }
                }
            }
        }

        public static string Utf8ToGB2312(string utf8String)
        {
            var fromEncoding = Encoding.UTF8;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var toEncoding = Encoding.GetEncoding("GB2312");
            return EncodingConvert(utf8String, fromEncoding, toEncoding);
        }

        public static string EncodingConvert(string fromString, Encoding fromEncoding, Encoding toEncoding)
        {
            byte[] fromBytes = fromEncoding.GetBytes(fromString);
            byte[] toBytes = Encoding.Convert(fromEncoding, toEncoding, fromBytes);

            string toString = toEncoding.GetString(toBytes);
            return toString;
        }
    }
}
