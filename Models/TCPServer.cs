using Org.BouncyCastle.Utilities.Net;
using ServerYourWorldMMORPG.Models.Packets;
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
        private TcpListener? _listener;
        private Thread? _listenThread;
        private CancellationTokenSource? _cancellationTokenSource;
        private Dictionary<string, TcpClient> _connectedClients = new Dictionary<string, TcpClient>();
        public string IpString { get; private set; }
        public int Port { get; private set; }
        public int MaxPlayers { get; private set; }
        public bool isServerRunning { get; private set; }

        public TCPServer(string IpString, int port, int maxPlayers)
        {
            this.isServerRunning = false;
            this.IpString = IpString;
            this.Port = port;
            this.MaxPlayers = maxPlayers;
        }

        public void StartInBackground()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _listenThread = new Thread(() => StartListeningAsync(_cancellationTokenSource.Token).Wait());
            _listenThread.Start();
        }

        public void Stop()
        {
            foreach (var client in _connectedClients.Values)
            {
                client.Close();
            }

            _cancellationTokenSource?.Cancel();
            _listenThread?.Join();
            ConsoleUtility.Print("TCP Server has stopped!");
        }

        public async Task StartListeningAsync(CancellationToken cancellationToken)
        {
            IPAddress ipAddress = IPAddress.Parse(IpString);

            _listener = new TcpListener(ipAddress, Port);
            _listener.Start();
            ConsoleUtility.Print("TCP Server started at port: " + Port);

            while (!cancellationToken.IsCancellationRequested)
            {
                isServerRunning = true;
                TcpClient client = await _listener.AcceptTcpClientAsync();
                ConsoleUtility.Print("Client connected: " + client.Client.RemoteEndPoint);

                string clientId = Guid.NewGuid().ToString(); // Generate a unique identifier
                _connectedClients[clientId] = client;

                await ReceiveDataAsync(clientId, client);
            }
        }

        //public void BroadcastMessage(PacketBase packet)
        //{
        //    foreach (var client in _connectedClients.Values)
        //    {
        //        SendPacketToClient(packet, client);
        //    }
        //}

        //public void SendPacketToClient()
        //{
        //    try
        //    {
                
        //    }
        //}

        private async Task ReceiveDataAsync(string clientId, TcpClient client)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    ConsoleUtility.Print($"Received data from client {clientId}: {data}");

                    await ProcessDataAsync(clientId, data);
                }
            }
            catch (Exception ex)
            {
                ConsoleUtility.Print($"Error for client {clientId}: {ex.Message}");
                _connectedClients.Remove(clientId);
            }
        }

        private async Task ProcessDataAsync(string clientId, string data)
        {
            // Process the received data
            // ... (your game logic)

            // Example: Sending a response to the client
            await SendTcpDataAsync(clientId, "Hello, client " + clientId);
        }

        private async Task SendTcpDataAsync(string clientId, string message)
        {
            if (_connectedClients.TryGetValue(clientId, out TcpClient? client))
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.ASCII.GetBytes(message);
                    //stream.Write(data, 0, data.Length);
                    await stream.WriteAsync(data, 0, data.Length);
                    ConsoleUtility.Print($"Sent TCP data to client {clientId}: {message}");
                }
                catch (Exception ex)
                {
                    ConsoleUtility.Print($"TCP Error for client {clientId}: {ex.Message}");
                }
            }
        }

        public List<Client> GetConnectedClients()
        {
            return _connectedClients.Values.Select(client => new Client
            {
                Id = client.Client.RemoteEndPoint.ToString(),
                IP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(),
                Port = ((IPEndPoint)client.Client.RemoteEndPoint).Port
            }).ToList();
        }
    }
}
