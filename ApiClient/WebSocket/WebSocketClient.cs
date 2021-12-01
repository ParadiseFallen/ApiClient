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
using ApiClient.Interfaces;

namespace ApiClient.WebSocket
{
    public class WebSocketClient : IAsyncDisposable, IWebSocketMessageFactory
    {
        #region Properties

        public Uri Uri { get; init; }
        private Pipe RecivePipe { get; init; }
        public int ReciveBufferSize { get; set; } = 2048;

        private ClientWebSocket WebSocket { get; set; } = default!;
        private Func<ClientWebSocket> WebSocketBuilder { get; init; }
        private IWebSocketMessageFactory MessageFactory { get; init; }
        private Subject<(WebSocketClient Sender,WebSocketMessage Message)> MessageRecivedSubject { get; init; } = new();

        #region Computed
        public WebSocketState State => WebSocket.State;

        public IObservable<(WebSocketClient Sender, WebSocketMessage Message)> MessageRecived => MessageRecivedSubject.AsObservable();

        #endregion

        #endregion

        public WebSocketClient(
            Uri uri,
            IWebSocketMessageFactory messageFactory,
            Func<ClientWebSocket> wsBuilder = null,
            PipeOptions pipeOptions = null)
        {
            Uri = uri;
            MessageFactory = messageFactory;
            WebSocketBuilder = wsBuilder ?? (() => new ClientWebSocket());
            var defaultOptions = new PipeOptions(minimumSegmentSize: ReciveBufferSize, pauseWriterThreshold: 0, resumeWriterThreshold: 0);
            RecivePipe = new Pipe(pipeOptions ?? defaultOptions);
        }


        public async Task Connect(CancellationToken cancellationToken)
        {
            WebSocket = WebSocketBuilder();
            await WebSocket.ConnectAsync(Uri, cancellationToken).ConfigureAwait(false);
        }

        public async Task Start(TaskScheduler taskScheduler = null,CancellationToken cancellationToken = default)
        {
            var listenTask =  Task
                .Factory
                .StartNew(ListenWebSocket, cancellationToken,
                TaskCreationOptions.LongRunning,
                taskScheduler);
            await listenTask;
        }

        public async Task Stop(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure,string message = null,CancellationToken cancellationToken = default)
        {
            await WebSocket.CloseAsync(closeStatus, message, cancellationToken);
        }

        #region Send

        public async Task Send(WebSocketMessage message, WebSocketMessageFlags messageFlags = WebSocketMessageFlags.EndOfMessage, CancellationToken cancellationToken = default)
        {
            await WebSocket.SendAsync(message.Data, message.Type, messageFlags, cancellationToken);
        }

        #endregion

        private async Task ListenWebSocket()
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
                    var buffer = new byte[readResult.Buffer.Length];
                    readResult.Buffer.CopyTo(buffer);
                    reader.AdvanceTo(readResult.Buffer.End);
                    //var x = (Sender: this, Message: MessageFactory.Create(buffer, result.MessageType);
                    MessageRecivedSubject.OnNext((this,MessageFactory.CreateMessage(buffer, result.MessageType)));
                    return;
                }
                // implicit else
                // TODO: handle zero-value message
                Console.WriteLine($"Zero value was receved");
            }
            catch (Exception)
            {
                // TODO: hande message read exception
                throw;
            }
        }
        #region IAsyncDisposible
        public async ValueTask DisposeAsync()
        {
            await RecivePipe.Reader.CompleteAsync().ConfigureAwait(false);
            await RecivePipe.Writer.CompleteAsync().ConfigureAwait(false);
            WebSocket?.Abort();
            WebSocket?.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion

        #region IWebSocketMessageFactory
        
        public WebSocketMessage CreateMessage(byte[] data, WebSocketMessageType type = WebSocketMessageType.Binary) => MessageFactory.CreateMessage(data,type);

        public WebSocketMessage CreateMessage(string message, WebSocketMessageType type = WebSocketMessageType.Text) => MessageFactory.CreateMessage(message, type);
        
        public WebSocketMessage CreateMessage<T>(T data, WebSocketMessageType type = WebSocketMessageType.Binary) => MessageFactory.CreateMessage(data, type);

        public T FromMessage<T>(WebSocketMessage message) => MessageFactory.FromMessage<T>(message);

        #endregion
    }
}
