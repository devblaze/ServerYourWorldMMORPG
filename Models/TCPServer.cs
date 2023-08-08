using Org.BouncyCastle.Utilities.Net;
using ServerYourWorldMMORPG.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using IPAddress = System.Net.IPAddress;

namespace ServerYourWorldMMORPG.Models
{
    public class TCPServer
    {
        private TcpListener? listener;
        private Thread? listenThread;
        private CancellationTokenSource? cancellationTokenSource;
        private Dictionary<string, TcpClient> connectedClients = new Dictionary<string, TcpClient>();
        public string IpString { get; private set; }
        public int Port { get; private set; }
        public int MaxPlayers { get; private set; }
        public bool isServerRunning { get; private set; }

        public TCPServer(string IpString, int port, int maxPlayers)
        {
            isServerRunning = false;
            this.IpString = IpString;
            Port = port;
            MaxPlayers = maxPlayers;
        }

        public void StartInBackground()
        {
            cancellationTokenSource = new CancellationTokenSource();
            listenThread = new Thread(() => StartListening(cancellationTokenSource.Token));
            listenThread.Start();
        }

        public void Stop()
        {
            foreach (var client in connectedClients.Values)
            {
                client.Close();
            }
            cancellationTokenSource?.Cancel();
            listenThread?.Join();
            ConsoleUtility.Print("TCP Server has stopped!");
        }

        public void StartListening(CancellationToken cancellationToken)
        {
            IPAddress ipAddress = IPAddress.Parse(IpString);

            listener = new TcpListener(ipAddress, Port);
            listener.Start();
            ConsoleUtility.Print("TCP Server started at port: " + Port);

            while (!cancellationToken.IsCancellationRequested)
            {
                isServerRunning = true;
                TcpClient client = listener.AcceptTcpClient();
                ConsoleUtility.Print("Client connected: " + client.Client.RemoteEndPoint);

                string clientId = Guid.NewGuid().ToString(); // Generate a unique identifier
                connectedClients[clientId] = client;

                ReceiveData(clientId, client);
            }
        }

        private void ReceiveData(string clientId, TcpClient client)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    ConsoleUtility.Print($"Received data from client {clientId}: {data}");

                    ProcessData(clientId, data);
                }
            }
            catch (Exception ex)
            {
                ConsoleUtility.Print($"Error for client {clientId}: {ex.Message}");
                connectedClients.Remove(clientId);
            }
        }

        private void ProcessData(string clientId, string data)
        {
            // Process the received data
            // ... (your game logic)

            // Example: Sending a response to the client
            SendTcpData(clientId, "Hello, client " + clientId);
        }

        private void SendTcpData(string clientId, string message)
        {
            if (connectedClients.TryGetValue(clientId, out TcpClient? client))
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.ASCII.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    ConsoleUtility.Print($"Sent TCP data to client {clientId}: {message}");
                }
                catch (Exception ex)
                {
                    ConsoleUtility.Print($"TCP Error for client {clientId}: {ex.Message}");
                }
            }
        }
    }
}
