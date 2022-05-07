using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;

namespace WebCastFeed.WebSocket
{
    public class WebSocketTransport : IAsyncTransport
    {
        private readonly string _WebSocketUrl;
        private readonly int _MaxRetry;

        private const int WS_KEEP_ALIVE_TIMEOUT_SECONDS = 60;

        private readonly TimeSpan CONNECT_RETRY_WAIT_TIME = new TimeSpan(0, 0, 0, 0, 500);

        private readonly TimeSpan RECONNECT_RETRY_WAIT_TIME = new TimeSpan(0, 0, 0, 0, 100);

        private const int BUFFER_SIZE = 4096;

        private const int MAX_SEND_CHUNK_SIZE = 4096;

        private const int MAX_RECONNECT_RETRIES = 5;

        private int _ReconnectRetries;

        private bool _Stopped;

        private System.Net.WebSockets.WebSocket _WebSocket; 

        public event EventHandler OnReady;

        public event EventHandler<TransportClosedEventArgs> OnClosed;

        public event EventHandler<TransportReconnectEventArgs> OnReconnect;

        public event EventHandler<MessageEventArgs> OnMessage;

        private SemaphoreSlim _WebsocketLock = new SemaphoreSlim(1, 1);

        private AsyncReaderWriterLock _ReconnectLock = new AsyncReaderWriterLock();

        public WebSocketTransport(string uriWebSocketUrl, int port, int maxRetry)
        {
            _WebSocketUrl = uriWebSocketUrl;

            if (maxRetry <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetry));
            }

            _MaxRetry = maxRetry;
        }

        internal bool IsReady => (_WebSocket != null) && (_WebSocket.State == WebSocketState.Open);

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _WebsocketLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                for (var i = 0; i < _MaxRetry; i++)
                {
                    try
                    {
                        await ConnectAsync(_WebSocketUrl, cancellationToken).ConfigureAwait(false);
                        break;
                    }
                    catch (Exception e)
                    {
                        if (i >= _MaxRetry - 1)
                        {
                            Console.WriteLine(e);
                            throw e;
                        }

                        await Task.Delay(CONNECT_RETRY_WAIT_TIME).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _WebsocketLock.Release();
            }

            var listenThread = new Thread(async o =>
            {
                await ListenAsync(cancellationToken).ConfigureAwait(false);
            });
            listenThread.Name = "WebSocket_Listen_Thread";
            listenThread.IsBackground = true;
            listenThread.Start();

            OnReady?.Invoke(this, EventArgs.Empty);
        }

        private async Task ListenAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting to listen for messages from WS transport...");
            _Stopped = false;
            var buffer = new byte[BUFFER_SIZE];
            try
            {
                _ReconnectRetries = 0;

                while (!_Stopped)
                {
                    while (IsReady)
                    {
                        await ReadNextMessage(buffer, cancellationToken).ConfigureAwait(false);
                    }

                    if (_Stopped)
                    {
                        break;
                    }

                    if (_ReconnectRetries >= MAX_RECONNECT_RETRIES)
                    {
                        // don't try to reconnect any more
                        Console.WriteLine($"Failed reconnecting after {_ReconnectRetries} retries.");
                        break;
                    }

                    _ReconnectRetries++;
                    Console.WriteLine($"Websocket closed, attempt {_ReconnectRetries} to reconnect websocket...");

                    if (_ReconnectRetries > 1)
                    {
                        // give it some time if the first retry failed
                        Console.WriteLine($"Waiting {RECONNECT_RETRY_WAIT_TIME.TotalMilliseconds}ms before retry #{_ReconnectRetries}");
                        await Task.Delay(RECONNECT_RETRY_WAIT_TIME).ConfigureAwait(false);
                    }

                    await ReconnectAsync(cancellationToken).ConfigureAwait(false);
                }

                Console.WriteLine($"Websocket transport loop ended! Websocket state is now {_WebSocket?.State}");
            }
            catch (Exception e)
            {
                // Don't allow exceptions to escape as this could crash the app before cleaning up
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("Disposing websocket transport...");
                DisposeWebSocket();

                // signal that the transport has been closed
                OnClosed?.Invoke(this, new TransportClosedEventArgs());
            }
        }

        private async Task ReadNextMessage(byte[] buffer, CancellationToken cancellationToken)
        {
            try
            {
                var byteSegment = new ArraySegment<byte>(buffer);
                string stringResult = "";
                WebSocketReceiveResult result;
                MemoryStream ms = null;
                while (true)
                {
                    result = await _WebSocket.ReceiveAsync(byteSegment, cancellationToken).ConfigureAwait(false);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        // Throw an exception to prevent further processing of the message
                        throw new Exception("Received a 'Close' websocket message from WebSocket server forcing reconnect.");
                    }
                    else if (result.MessageType != WebSocketMessageType.Text)
                    {
                        Console.WriteLine("Received a non-text web socket message from WebSocket server!");
                        break;
                    }
                    else if (result.EndOfMessage)
                    {
                        if (ms != null)
                        {
                            //append the last bytes and read the stream to string
                            ms.Write(buffer, byteSegment.Offset, result.Count);
                            ms.Seek(0, SeekOrigin.Begin);
                            using (var reader = new StreamReader(ms, Encoding.UTF8))
                            {
                                stringResult = reader.ReadToEnd();
                            }
                            ms.Dispose();
                        }
                        else
                        {
                            stringResult = Encoding.UTF8.GetString(buffer, byteSegment.Offset, result.Count);
                        }
                        break;
                    }
                    else
                    {
                        //store all current bytes and continue fetching more
                        if (ms == null)
                        {
                            ms = new MemoryStream(buffer.Length * 2);
                        }
                        ms.Write(buffer, byteSegment.Offset, result.Count);
                    }
                }

                if (!string.IsNullOrWhiteSpace(stringResult))
                {
                    // execute on a thread pool thread to free the listening loop
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        var msg = o.ToString();
                        OnMessage?.Invoke(this, new MessageEventArgs(msg));
                    }, stringResult);
                }
                else
                {
                    Console.WriteLine("Received an empty websocket message from WebSocket server!");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in websocket transport loop: {e}");
            }
        }

        private async Task ReconnectAsync(CancellationToken cancellationToken)
        {
            await _WebsocketLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await ConnectAsync(_WebSocketUrl, cancellationToken).ConfigureAwait(false);

                // This should happen in a separate thread to allow
                // the listen thread to continue processing incoming messages.
                Thread t = new Thread(async (a) =>
                {
                    // This will have to wait for any in-flight messages to be sent
                    await _ReconnectLock.EnterWriteLockAsync(cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var args = new TransportReconnectEventArgs();
                        OnReconnect?.Invoke(this, args);
                        if (args.IsFailed)
                        {
                            //failed to reconnect
                            DisposeWebSocket();
                        }
                        else
                        {
                            Console.WriteLine("Websocket connection to server reconnected successfully!");
                            _ReconnectRetries = 0;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error reconnecting to server {e}");
                    }
                    finally
                    {
                        _ReconnectLock.ExitWriteLock();
                    }
                });
                t.Name = "Reconnect_Thread";
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception in attempt to reconnect a websocket connection: {e}");
                DisposeWebSocket();
            }
            finally
            {
                _WebsocketLock.Release();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Websocket transport received a request to stop. Disposing the connection and shutting down...");

            _Stopped = true;

            await _WebsocketLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await _WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "close", cancellationToken)
                    .ContinueWith((t, o) =>
                    {
                        if (t.IsFaulted)
                        {
                            ((System.Net.WebSockets.WebSocket)o).Dispose();
                        }
                    }, _WebSocket).ConfigureAwait(false);
            }
            finally
            {
                _WebSocket = null;
                _WebsocketLock.Release();
            }

            _WebsocketLock.Dispose();
            _ReconnectLock.Dispose();
        }

        public async Task SendAsync(List<DouyinMessage> request, CancellationToken cancellationToken)
        {
            try
            {
                // waits if websocket is locked for connecting, sending another message etc.
                await _WebsocketLock.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    if (!IsReady)
                    {
                        throw new TransportNotReadyException($"Websocket is not in a ready state [state={_WebSocket?.State}]!");
                    }
                    string message = SerializeRequest(request);
                    var messageBuffer = Encoding.UTF8.GetBytes(message);
                    var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / MAX_SEND_CHUNK_SIZE);
                    int lastMessage = messagesCount - 1;
                    try
                    {
                        // chunkify the message if it's too big
                        for (var i = 0; i < messagesCount; i++)
                        {
                            var offset = MAX_SEND_CHUNK_SIZE * i;
                            var count = MAX_SEND_CHUNK_SIZE;

                            if ((count * (i + 1)) > messageBuffer.Length)
                            {
                                count = messageBuffer.Length - offset;
                            }

                            await _WebSocket.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count), WebSocketMessageType.Text, i == lastMessage, cancellationToken).ConfigureAwait(false);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed sending {messagesCount} chunks for message \"{message}\" of {messageBuffer.Length} bytes on websocket: {e}");
                        throw new Exception("Failed sending websocket message");
                    }
                }
                finally
                {
                    _WebsocketLock.Release();
                }
            }
            finally
            {
                _ReconnectLock.ExitReadLock();
            }
        }

        private string SerializeRequest(List<DouyinMessage> request)
        {
            return JsonSerializer.Serialize(request);
        }

        private async Task ConnectAsync(string uri, CancellationToken cancellationToken)
        {
            var cws = new ClientWebSocket();
            cws.Options.KeepAliveInterval = TimeSpan.FromSeconds(WS_KEEP_ALIVE_TIMEOUT_SECONDS);

            var wsUri = new Uri(uri);
            await cws.ConnectAsync(wsUri, cancellationToken).ConfigureAwait(false);

            DisposeWebSocket();
            _WebSocket = cws;
        }

        private void DisposeWebSocket()
        {
            _WebSocket?.Abort();
            _WebSocket?.Dispose();
            _WebSocket = null;
        }
    }
}
