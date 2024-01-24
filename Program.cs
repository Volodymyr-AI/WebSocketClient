using MessagePack;
using System.Net.WebSockets;
using System.Text;

namespace WebSocketClient
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            ClientWebSocket client = new ClientWebSocket();
            Uri serverUri = new Uri("ws://192.168.0.200:5000/ws");

            await client.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("Connected to the server");

            
            var messageToSend = MessagePackSerializer.Serialize(new Message { Content = "Hello from client" });
            await client.SendAsync(new ArraySegment<byte>(messageToSend), WebSocketMessageType.Binary, true, CancellationToken.None);
            Console.WriteLine("Message sent to the server");

            
            var buffer = new byte[1024];
            WebSocketReceiveResult result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var receivedMessage = MessagePackSerializer.Deserialize<Message>(new ArraySegment<byte>(buffer, 0, result.Count).ToArray());
            Console.WriteLine("Received from server: " + receivedMessage.Content);

            
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
    }

    [MessagePackObject]
    public class Message
    {
        [Key(0)]
        public string Content { get; set; }
    }
}
