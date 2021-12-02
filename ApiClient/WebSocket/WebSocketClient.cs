using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using ApiClient.Data.Records;
using ApiClient.Interfaces.WebSocket;

using Microsoft.Extensions.Logging;

using Polly;

namespace ApiClient.WebSocket
{
    public class WebSocketClient : IAsyncDisposable
    {
        #region Properties

        public Uri Uri { get; init; }
        public int ReciveBufferSize { get; set; } = 2048;
        public IWebSocketMessageConverter MessageConvereter { get; init; }

        protected bool IsStarted { get; set; } = false;

        protected Pipe RecivePipe { get; init; }
        protected AsyncPolicy ExecutionPolicy { get; init; }
        protected ClientWebSocket WebSocket { get; set; } = default!;
        protected Func<ClientWebSocket> WebSocketBuilder { get; init; }
        protected ILogger<WebSocketClient> Logger { get; init; }

        protected Subject<(IWebSocketMessageConverter MessageFactory, WebSocketMessage Message)> MessageRecivedSubject { get; init; } =
            new();

        protected Subject<Exception> ExceptionSubject { get; init; } =
            new();

        protected event Action OnConnected;

        protected event Action OnDisconected;

        #region Computed
        public WebSocketState State => WebSocket.State;

        public IObservable<(IWebSocketMessageConverter MessageFactory, WebSocketMessage Message)> MessageRecived =>
            MessageRecivedSubject.AsObservable();

        public IObservable<Exception> OnException =>
            ExceptionSubject.AsObservable();


        #endregion



        #endregion

        public WebSocketClient(
            Uri uri,
            IWebSocketMessageConverter messageFactory,
            Func<ClientWebSocket> wsBuilder = null,
            AsyncPolicy executionPolicy = null,
            PipeOptions pipeOptions = null,
            ILogger<WebSocketClient> logger = null)
        {
            Uri = uri;
            MessageConvereter = messageFactory;
            Logger = logger;
            WebSocketBuilder = wsBuilder ?? (() => new ClientWebSocket());

            ExecutionPolicy = executionPolicy ?? Policy.NoOpAsync();

            var defaultOptions = new PipeOptions(
                minimumSegmentSize: ReciveBufferSize,
                pauseWriterThreshold: 0,
                resumeWriterThreshold: 0);

            RecivePipe = new Pipe(pipeOptions ?? defaultOptions);
        }




        public async Task Start(CancellationToken cancellationToken = default)
        {
            if (IsStarted)
                throw new InvalidOperationException("Start already called");

            IsStarted = true;

            await ExecutionPolicy.ExecuteAsync(async (token) =>
            {
                try
                {
                    if (WebSocket is null || State != WebSocketState.Open)
                        await Reconnect(token);
                    await ListenWebSocket(token);
                }
                catch (Exception ex)
                {
                    ExceptionSubject.OnNext(ex);
                    Logger.LogError(ex, "Exception in main loop");
                    throw;
                }
            }, cancellationToken);
        }

        public async Task Stop(
            WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure,
            string message = null,
            CancellationToken cancellationToken = default)
        {

            await WebSocket.CloseAsync(closeStatus, message, cancellationToken).ConfigureAwait(false);
            WebSocket = null;
            IsStarted = false;
        }

        public async Task Send(
            WebSocketMessage message,
            WebSocketMessageFlags messageFlags = WebSocketMessageFlags.EndOfMessage,
            CancellationToken cancellationToken = default) =>
                await WebSocket.SendAsync(message.Data, message.Type, messageFlags, cancellationToken).ConfigureAwait(false);

        protected async Task Connect(CancellationToken cancellationToken = default)
        {
            WebSocket = WebSocketBuilder();
            await WebSocket.ConnectAsync(Uri, cancellationToken).ConfigureAwait(false);
            OnConnected?.Invoke();
        }

        protected async Task Reconnect(CancellationToken cancellationToken = default)
        {
            WebSocket?.Abort();
            WebSocket?.Dispose();
            await Connect(cancellationToken);
        }

        private async Task ListenWebSocket(CancellationToken cancellationToken = default)
        {
            var writer = RecivePipe.Writer;
            try
            {
                while (State == WebSocketState.Open)
                {
                    var result = await WebSocket.ReceiveAsync(
                        writer.GetMemory(ReciveBufferSize),
                        cancellationToken)
                            .ConfigureAwait(false);
                    // say how much bytes was written
                    writer.Advance(result.Count);

                    if (result.EndOfMessage)
                    {
                        // flush n fire event
                        await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            OnDisconected?.Invoke();
                            return;
                        }
                        else
                            NotifyMessageRecived(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, $"Error while listining ClientWebSocket");
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
                    MessageRecivedSubject.OnNext((MessageConvereter, MessageConvereter.CreateMessage(buffer, result.MessageType)));
                    return;
                }
                // implicit else
                Logger?.LogInformation($"Message with zero payload recived. [{result.MessageType}]");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, $"Error while reading recived message");
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
            ExceptionSubject.OnCompleted();
            MessageRecivedSubject.OnCompleted();
            ExceptionSubject.Dispose();
            MessageRecivedSubject.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
