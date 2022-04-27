using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;
using System.Text.Json;

namespace WebCastFeed.Operations
{
    public class HandleLiveFeedOperation : IAsyncOperation<List<DouyinMessage>, bool>
    {
        public async ValueTask<bool> ExecuteAsync(List<DouyinMessage> input, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("A handle live feed request is started");
            var webSocketServerAddress = Environment.GetEnvironmentVariable("WebSocketServerAddress") ?? "ws://localhost:5000";

            Console.WriteLine($"ws server address: {webSocketServerAddress}");
            using (ClientWebSocket client = new ClientWebSocket())
            {
                var serviceUri = new Uri(webSocketServerAddress);
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(1));
                try
                {
                    await client.ConnectAsync(serviceUri, cts.Token);
                    var messageString = JsonSerializer.Serialize(input);
                    var byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageString));
                    await client.SendAsync(byteToSend, WebSocketMessageType.Text, true, cts.Token);
                    await client.CloseAsync(WebSocketCloseStatus.Empty, "closed", CancellationToken.None);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"exception while handling live feed request {e}");
                    Console.WriteLine(e);
                }
            }
            return true;
        }
    }
}
