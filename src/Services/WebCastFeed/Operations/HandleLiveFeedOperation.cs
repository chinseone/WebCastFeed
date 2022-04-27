﻿using System;
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

                    await Task.WhenAll(Receive(client), Send(client, messageString));
                    // while (client.State == WebSocketState.Open)
                    // {
                    //     await client.SendAsync(byteToSend, WebSocketMessageType.Text, true, cts.Token);
                    //     // var responseBuffer = new byte[1024];
                    //     // var offset = 0;
                    //     // var packet = 1024;
                    //     // while (true)
                    //     // {
                    //     //     var byteReceived = new ArraySegment<byte>(responseBuffer, offset, packet);
                    //     //     var response = await client.ReceiveAsync(byteReceived, cts.Token);
                    //     //     var responseMessage = Encoding.UTF8.GetString(responseBuffer, offset, response.Count);
                    //     //     if (response.EndOfMessage)
                    //     //     {
                    //     //         break;
                    //     //     }
                    //     // }
                    // }
                    // await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "closed", CancellationToken.None);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"exception while handling live feed request {e}");
                    Console.WriteLine(e);
                }
            }
            return true;
        }

        private static async Task Send(ClientWebSocket ws, string input)
        {
            var messageString = JsonSerializer.Serialize(input);
            var byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageString));

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
