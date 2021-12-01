using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using ApiClient.Data.Records;

namespace ApiClient.WebSocket
{
    public class WebSocketClient : IAsyncDisposable
    {
        //public JsonSerializerOptions JsonSerializerOptions { get; set; }

        #region Properties

        public Uri Uri { get; init; }
        private Pipe RecivePipe { get; init; }
        public int ReciveBufferSize { get; set; } = 2048;

        private ClientWebSocket WebSocket { get; set; } = default!;
        private Func<ClientWebSocket> WebSocketBuilder { get; init; }
        private Subject<WebSocketMessage> MessageRecivedSubject { get; init; } = new();

        #region Computed
        public WebSocketState State => WebSocket.State;

        public IObservable<WebSocketMessage> MessageRecived => MessageRecivedSubject.AsObservable();

        #endregion

        #endregion

        public WebSocketClient(Uri uri, Func<ClientWebSocket>? wsBuilder = null, JsonSerializerOptions? jsonSerializerOptions = null, PipeOptions? pipeOptions = null)
        {
            Uri = uri;
            WebSocketBuilder = wsBuilder ?? (() => new ClientWebSocket());
            var defaultOptions = new PipeOptions(minimumSegmentSize: ReciveBufferSize, pauseWriterThreshold: 0, resumeWriterThreshold: 0);
            RecivePipe = new Pipe(pipeOptions ?? defaultOptions);
            //JsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);
        }


        public async Task Connect(CancellationToken cancellationToken)
        {
            WebSocket = WebSocketBuilder();
            await WebSocket.ConnectAsync(Uri, cancellationToken).ConfigureAwait(false);
        }

        public async Task Start()
        {
            // add send loop
            await Task.WhenAll(Recive());
        }

        #region Send

        public async Task Send(WebSocketMessage message, WebSocketMessageFlags messageFlags = WebSocketMessageFlags.EndOfMessage, CancellationToken cancellationToken = default)
        {
            await WebSocket.SendAsync(message.Data, message.Type, messageFlags, cancellationToken);
        }

        //public async Task Send(byte[] data, WebSocketMessageType messageType = WebSocketMessageType.Binary, WebSocketMessageFlags messageFlags = WebSocketMessageFlags.EndOfMessage,CancellationToken cancellationToken = default)
        //{
        //    await WebSocket.SendAsync(data, messageType, messageFlags, cancellationToken);
        //}

        //public async Task Send(string text, WebSocketMessageType messageType = WebSocketMessageType.Text, WebSocketMessageFlags messageFlags = WebSocketMessageFlags.EndOfMessage, CancellationToken cancellationToken = default)
        //{
        //    await WebSocket.SendAsync(Encoding.UTF8.GetBytes(text), messageType, messageFlags, cancellationToken);
        //}

        //public async Task Send<T>(T data, WebSocketMessageType messageType = WebSocketMessageType.Text, WebSocketMessageFlags messageFlags = WebSocketMessageFlags.EndOfMessage, CancellationToken cancellationToken = default)
        //{
        //    var messageData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, JsonSerializerOptions));
        //    await WebSocket.SendAsync(messageData, messageType, messageFlags, cancellationToken);
        //}

        #endregion

        private async Task Recive()
        {
            var writer = RecivePipe.Writer;

            try
            {
                while (State == WebSocketState.Open)
                {
                    var result = await WebSocket.ReceiveAsync(writer.GetMemory(ReciveBufferSize), CancellationToken.None).ConfigureAwait(false);
                    // say how much bytes was written
                    writer.Advance(result.Count);

                    if (result.EndOfMessage)
                    {
                        // flush n fire event
                        await writer.FlushAsync().ConfigureAwait(false);
                        NotifyMessageRecived(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private void NotifyMessageRecived(ValueWebSocketReceiveResult result)
        {
            var reader = RecivePipe.Reader;
            try
            {
                if (reader.TryRead(out var readResult) && readResult.Buffer.Length > 0)
                {
                    // allocate buffer with size of message
                    var buffer = new byte[readResult.Buffer.Length];
                    // copy message bytes to buffer
                    readResult.Buffer.CopyTo(buffer);
                    // advance memory in pipe
                    reader.AdvanceTo(readResult.Buffer.End);
                    // create message event args and fire message event args
                    MessageRecivedSubject.OnNext(new WebSocketMessage(buffer, result.MessageType));
                    return;
                }
                // implicit else
                // handle zero-value message
                Console.WriteLine($"Zero value was receved");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await RecivePipe.Reader.CompleteAsync().ConfigureAwait(false);
            await RecivePipe.Writer.CompleteAsync().ConfigureAwait(false);
            WebSocket?.Abort();
            WebSocket?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
