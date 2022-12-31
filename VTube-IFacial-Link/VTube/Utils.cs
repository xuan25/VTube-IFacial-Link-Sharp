using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using VTube.DataModel;

namespace VTube
{
    static class Utils
    {
        private static readonly JsonSerializerOptions serializeOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public static void SendRequest<T>(ClientWebSocket clientWebSocket, T request) where T : RequestBase
        {
            string reqJsonStr = JsonSerializer.Serialize(request, serializeOptions);
            byte[] payload = Encoding.UTF8.GetBytes(reqJsonStr);
            clientWebSocket.SendAsync(payload, WebSocketMessageType.Text, true, CancellationToken.None).Wait();
        }

        public static T ReceiveResponse<T>(ClientWebSocket clientWebSocket) where T : ResponseBase
        {
            byte[] buffer = new byte[8192];
            WebSocketReceiveResult receiveResult;
            MemoryStream outputStream = new(8192);
            do
            {
                receiveResult = clientWebSocket.ReceiveAsync(buffer, CancellationToken.None).Result;
                if (receiveResult.MessageType != WebSocketMessageType.Close)
                    outputStream.Write(buffer, 0, receiveResult.Count);
            }
            while (!receiveResult.EndOfMessage);
            outputStream.Position = 0;

            T obj = JsonSerializer.Deserialize<T>(outputStream, serializeOptions);
            return obj;
        }
    }
}
