using System.Net.WebSockets;

namespace ApiClient.Data.Records
{
    public record WebSocketMessage
    {
        public byte[] Data { get; init; }
        public WebSocketMessageType Type { get; init; }

        public WebSocketMessage() { }
        public WebSocketMessage(byte[] data = null, WebSocketMessageType type = WebSocketMessageType.Binary)
        {
            Data = data;
            Type = type;
        }
    }
}
