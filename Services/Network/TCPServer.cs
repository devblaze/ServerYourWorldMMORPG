using ServerYourWorldMMORPG.Models.Network;
using ServerYourWorldMMORPG.Services.Interfaces;
using ServerYourWorldMMORPG.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text;
using IPAddress = System.Net.IPAddress;

namespace ServerYourWorldMMORPG.Services.Network
{
    public class TCPServer : ITCPServer
    {
        public string ServerIp { get; private set; }
        public int ServerPort { get; private set; }
        public int MaxConnectedClients { get; private set; }
        public bool IsServerRunning { get; private set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }

        private TcpListener? _listener;
        private Thread? _listenThread;
        private Dictionary<string, TcpClient> _connectedClients = new Dictionary<string, TcpClient>();

        public TCPServer(string IpString, int port, int maxPlayers)
        {
            ApplicationSettings.LoadSettings();
            ServerIp = ApplicationSettings.IpAddress;
            ServerPort = ApplicationSettings.TcpPort;
            MaxConnectedClients = ApplicationSettings.MaxPlayers;
            IsServerRunning = false;
            CancellationTokenSource = new CancellationTokenSource();
        }

        //public void StartInBackground()
        //{
        //    _listenThread = new Thread(() => StartListeningAsync().Wait());
        //    _listenThread.Start();
        //}

        public async Task Stop()
        {
            //Task.Run(() => DisconnectClients(_connectedClients));
            //Task.Delay(2000);

            //_listener.Stop();
            //_listenThread.Interrupt();
            //_listenThread.Abort();
            //CancellationTokenSource.Cancel();
            //CancellationTokenSource.Dispose();
            //IsServerRunning = false;
            //ConsoleUtility.Print("TCP Server has stopped!");
        }

        public void StartListeningAsync()
        {
            IPAddress ipAddress = IPAddress.Parse(ServerIp);

            _listener = new TcpListener(ipAddress, ServerPort);
            _listener.Start();
            ConsoleUtility.Print("TCP Server started at port: " + ServerPort);

            while (!CancellationTokenSource.IsCancellationRequested)
            {
                //TcpClient client = _listener.AcceptTcpClientAsync();
                TcpClient client = _listener.AcceptTcpClient();
                ConsoleUtility.Print("Client connected: " + client.Client.RemoteEndPoint);

                string clientId = Guid.NewGuid().ToString(); // Generate a unique identifier
                _connectedClients[clientId] = client;

                Task.Run(() => ReceiveDataAsync(clientId, client), CancellationTokenSource.Token);
            }
        }

        public List<UserClient> GetConnectedTcpClients()
        {
            return _connectedClients.Values.Select(client => new UserClient
            {
                Id = client.Client.RemoteEndPoint.ToString(),
                IP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(),
                Port = ((IPEndPoint)client.Client.RemoteEndPoint).Port
            }).ToList();
        }

        public async Task DisconnectClients(Dictionary<string, TcpClient> clientsToDisconnect)
        {
            foreach (var client in clientsToDisconnect.Values)
            {
                client.Close();
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
                IsServerRunning = true;

                while (!CancellationTokenSource.IsCancellationRequested)
                {
                    //bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    ConsoleUtility.Print($"Received data from client {clientId}: {data}");

                    Task.Run(() => ProcessDataAsync(clientId, data), CancellationTokenSource.Token);
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
            await Task.Run(() => SendTcpDataAsync(clientId, "Hello, client " + clientId));
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
    }
}
