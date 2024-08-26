using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bowl.Common;
using Serilog;

namespace Bowl.Services.LocalSocket
{
    public class Service
    {
        class Information
        {
            [JsonPropertyName("action")]
            public string Action { get; set; }

            [JsonPropertyName("data")]
            public object? Data { get; set; }
        }

        static Errors Distribute(Information info)
        {
            switch(info.Action)
            {
                case "":
                default: return Errors.Dict[ErrorType.NoError];
            }
        }

        static void Handle(object obj)
        {
            Socket client = (Socket)obj;

            // Parse the received data
            byte[] buffer = new byte[1024];
            int length = client.Receive(buffer);
            string raw = Encoding.UTF8.GetString(buffer, 0, length);
            Information info = JsonSerializer.Deserialize<Information>(raw);

            // Handle
            var data = Distribute(info);

            // Send data
            var message = JsonSerializer.Serialize(data);
            byte[] result = Encoding.UTF8.GetBytes(message);
            client.Send(result);

            // Closing the connection
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        static public void Start()
        {
            // define the endpoint and create a socket
            int port = Utils.config.GetValue<int>("LocalSocketServicePort");
            IPEndPoint local = new IPEndPoint(IPAddress.Loopback, port);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(local);
                listener.Listen(10);    // Max length of listening queue

                Log.Information("Local Socket service is created.");

                while (true)
                {
                    Socket client = listener.Accept();
                    IPEndPoint remote = (IPEndPoint)client.RemoteEndPoint;

                    Log.Information($"Client [{remote.Address}:{remote.Port}] is connected.");

                    ThreadPool.QueueUserWorkItem(Handle, client);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
