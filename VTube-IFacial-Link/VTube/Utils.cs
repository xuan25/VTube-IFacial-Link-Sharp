using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using VTube.DataModel;

namespace VTube
{
    static class Utils
    {
        public static void SendRequest<T>(ClientWebSocket clientWebSocket, T request) where T : RequestBase
        {
            JsonSerializerOptions options = new()
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            string reqJsonStr = JsonSerializer.Serialize(request, options);
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

            JsonSerializerOptions options = new()
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            T obj = JsonSerializer.Deserialize<T>(outputStream, options);
            return obj;
        }
    }
}
