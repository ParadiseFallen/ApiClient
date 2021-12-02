using System.Net.WebSockets;

using ApiClient.Data.Records;

namespace ApiClient.Interfaces.WebSocket
{
    public interface IWebSocketMessageConverter
    {
        WebSocketMessage CreateMessage(byte[] data, WebSocketMessageType type = WebSocketMessageType.Binary);
        WebSocketMessage CreateMessage(string message, WebSocketMessageType type = WebSocketMessageType.Text);
        WebSocketMessage CreateMessage<T>(T data, WebSocketMessageType type = WebSocketMessageType.Binary);
        T ConvertTo<T>(WebSocketMessage message);
        string ConvertToString(WebSocketMessage message);
    }
}
