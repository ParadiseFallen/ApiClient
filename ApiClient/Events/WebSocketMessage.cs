using Microsoft.Toolkit.HighPerformance.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient.WebSocket.Events
{
    public record WebSocketMessage
    {
        public byte[]? Data { get; init; }
        public WebSocketMessageType Type { get; init; }

        public WebSocketMessage() { }
        public WebSocketMessage(byte[]? data = null,WebSocketMessageType type = WebSocketMessageType.Binary)
        {
            Data = data;
            Type = type;
        }
    }
}
