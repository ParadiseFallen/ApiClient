using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using ApiClient.Data.Records;
using ApiClient.Interfaces;

namespace ApiClient.Factories
{
    public class WebSocketMessageFactory : IWebSocketMessageFactory
    {
        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        public Encoding Encoding { get; set; } = Encoding.UTF8;


        public WebSocketMessage CreateMessage(byte[] data, WebSocketMessageType type = WebSocketMessageType.Binary) =>
            new(data, type);


        public WebSocketMessage CreateMessage(string message, WebSocketMessageType type = WebSocketMessageType.Text) =>
            new(Encoding.GetBytes(message), type);

        public WebSocketMessage CreateMessage<T>(T data, WebSocketMessageType type = WebSocketMessageType.Binary)=>
            new(Encoding.GetBytes(JsonSerializer.Serialize(data,JsonSerializerOptions)), type);

        public T FromMessage<T>(WebSocketMessage message) =>
            JsonSerializer.Deserialize<T>(Encoding.GetString(message.Data), JsonSerializerOptions);
    }
}
