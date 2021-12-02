using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using ApiClient.Data.Records;
using ApiClient.Interfaces.WebSocket;

namespace ApiClient.Factories
{
    public class WebSocketMessageConverter : IWebSocketMessageConverter
    {
        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public WebSocketMessageConverter() { }

        public WebSocketMessage CreateMessage(byte[] data, WebSocketMessageType type = WebSocketMessageType.Binary) =>
            new(data, type);

        public WebSocketMessage CreateMessage(string message, WebSocketMessageType type = WebSocketMessageType.Text) =>
            new(Encoding.GetBytes(message), type);

        public WebSocketMessage CreateMessage<T>(T data, WebSocketMessageType type = WebSocketMessageType.Binary) =>
            new(Encoding.GetBytes(JsonSerializer.Serialize(data, JsonSerializerOptions)), type);

        /// <summary>
        /// Convert <c>WebSocketMessage.Data</c> to <c>T</c> using <c>Encoding</c> and <c>JsonSerializerOptions</c>
        /// </summary>
        public T ConvertTo<T>(WebSocketMessage message) =>
            JsonSerializer.Deserialize<T>(Encoding.GetString(message.Data), JsonSerializerOptions);

        /// <summary>
        /// Convert <c>WebSocketMessage.Data</c> to <c>string</c> using <c>Encoding</c>
        /// </summary>
        public string ConvertToString(WebSocketMessage message) => Encoding.GetString(message.Data);

    }
}
