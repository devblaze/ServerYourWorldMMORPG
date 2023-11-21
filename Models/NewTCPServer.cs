//using System.Net;
//using System.Net.Sockets;
//using System.Text;

//namespace ServerYourWorldMMORPG.Models.Network
//{
//    public class NewTCPServer
//    {
//        public string ServerIp { get; private set; }
//        public int ServerPort { get; private set; }
//        public int MaxConnectedClients { get; private set; }
//        public bool IsServerRunning { get; private set; }

//        private TcpListener _listener;
//        private CancellationTokenSource _cancellationTokenSource;
//        private Dictionary<string, TcpClient> _connectedClients = new Dictionary<string, TcpClient>();

//        public NewTCPServer(string ip, int port, int maxPlayers)
//        {
//            ServerIp = ip;
//            ServerPort = port;
//            MaxConnectedClients = maxPlayers;
//            IsServerRunning = false;
//        }

//        public async Task StartListeningAsync(CancellationToken cancellationToken)
//        {
//            IPAddress ipAddress = IPAddress.Parse(ServerIp);

//            _listener = new TcpListener(ipAddress, ServerPort);
//            _listener.Start();
//            Console.WriteLine("TCP Server started at port: " + ServerPort);

//            while (!cancellationToken.IsCancellationRequested)
//            {
//                IsServerRunning = true;
//                TcpClient client = await _listener.AcceptTcpClientAsync();
//                Console.WriteLine("Client connected: " + client.Client.RemoteEndPoint);

//                string clientId = Guid.NewGuid().ToString(); // Generate a unique identifier
//                _connectedClients[clientId] = client;

//                _ = ReceiveDataAsync(clientId, client); // Start receiving data for this client
//            }
//        }

//        public List<UserClient> GetConnectedTcpClients()
//        {
//            return _connectedClients.Values.Select(client => new UserClient
//            {
//                Id = client.Client.RemoteEndPoint.ToString(),
//                IP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(),
//                Port = ((IPEndPoint)client.Client.RemoteEndPoint).Port
//            }).ToList();
//        }

//        public void Stop()
//        {
//            _listener.Stop();
//            _cancellationTokenSource.Cancel();
//            IsServerRunning = false;
//            Console.WriteLine("TCP Server has stopped!");
//        }

//        private async Task ReceiveDataAsync(string clientId, TcpClient client)
//        {
//            try
//            {
//                byte[] buffer = new byte[1024];
//                int bytesRead;
//                NetworkStream stream = client.GetStream();

//                while (true)
//                {
//                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, _cancellationTokenSource.Token);

//                    if (bytesRead == 0)
//                    {
//                        // No data received; the client has disconnected
//                        break;
//                    }

//                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
//                    Console.WriteLine($"Received data from client {clientId}: {data}");

//                    await ProcessDataAsync(clientId, data);
//                }
//            }
//            catch (OperationCanceledException)
//            {
//                // Cancellation requested; server stopping
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error for client {clientId}: {ex.Message}");
//            }
//            finally
//            {
//                _connectedClients.Remove(clientId);
//                client.Close();
//                Console.WriteLine($"Client {clientId} disconnected.");
//            }
//        }

//        private async Task ProcessDataAsync(string clientId, string data)
//        {
//            // Process the received data
//            // ... (your game logic)

//            // Example: Sending a response to the client
//            await SendTcpDataAsync(clientId, "Hello, client " + clientId);
//        }

//        private async Task SendTcpDataAsync(string clientId, string message)
//        {
//            if (_connectedClients.TryGetValue(clientId, out TcpClient client))
//            {
//                try
//                {
//                    NetworkStream stream = client.GetStream();
//                    byte[] data = Encoding.ASCII.GetBytes(message);
//                    await stream.WriteAsync(data, 0, data.Length, _cancellationTokenSource.Token);
//                    Console.WriteLine($"Sent TCP data to client {clientId}: {message}");
//                }
//                catch (OperationCanceledException)
//                {
//                    // Cancellation requested; server stopping
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"TCP Error for client {clientId}: {ex.Message}");
//                }
//            }
//        }
//    }
//}
