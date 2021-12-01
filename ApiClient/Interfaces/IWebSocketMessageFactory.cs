using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

using ApiClient.Data.Records;

namespace ApiClient.Interfaces
{
    public interface IWebSocketMessageFactory
    {
        WebSocketMessage CreateMessage(byte[] data, WebSocketMessageType type = WebSocketMessageType.Binary);
        WebSocketMessage CreateMessage(string message, WebSocketMessageType type = WebSocketMessageType.Text);
        WebSocketMessage CreateMessage<T>(T data, WebSocketMessageType type = WebSocketMessageType.Binary);
        T FromMessage<T>(WebSocketMessage message);
    }
}
