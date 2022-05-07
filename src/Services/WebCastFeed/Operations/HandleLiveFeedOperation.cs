using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;
using System.Text.Json;
using WebCastFeed.WebSocket;

namespace WebCastFeed.Operations
{
    public class HandleLiveFeedOperation : IAsyncOperation<List<DouyinMessage>, bool>
    {
        private readonly IWebSocketClient _WebSocketClient;

        public HandleLiveFeedOperation(IWebSocketClient webSocketClient)
        {
            _WebSocketClient = webSocketClient ?? throw new ArgumentNullException(nameof(webSocketClient));
        }

        public async ValueTask<bool> ExecuteAsync(List<DouyinMessage> input, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("A handle live feed request is started");
            var webSocketServerAddress = Environment.GetEnvironmentVariable("WebSocketServerAddress") ?? "ws://localhost:6000";

            Console.WriteLine($"ws server address: {webSocketServerAddress}");

            await _WebSocketClient.SendAsync(input);

            // using (ClientWebSocket client = new ClientWebSocket())
            // {
            //     var serviceUri = new Uri(webSocketServerAddress);
            //     var cts = new CancellationTokenSource();
            //     cts.CancelAfter(TimeSpan.FromSeconds(10));
            //     try
            //     {
            //         await client.ConnectAsync(serviceUri, cts.Token);
            //
            //         var messageString = JsonSerializer.Serialize(input);
            //
            //         await Task.WhenAll(Receive(client), Send(client, messageString));
            //     }
            //     catch (Exception e)
            //     {
            //         Console.WriteLine($"exception while handling live feed request {e}");
            //         Console.WriteLine(e);
            //     }
            // }
            return true;
        }

        private static async Task Send(ClientWebSocket ws, string input)
        {
            var byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(input));

            while (ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(byteToSend, WebSocketMessageType.Text, true, CancellationToken.None);
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
        }

        private static async Task Receive(ClientWebSocket ws)
        {
            byte[] buffer = new byte[4096];
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    Console.WriteLine("Received: " + Encoding.UTF8.GetString(buffer).TrimEnd('\0'));
                }
            }
        }
    }
}
