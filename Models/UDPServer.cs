using ServerYourWorldMMORPG.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerYourWorldMMORPG.Models
{
    public class UDPServer
    {
        private UdpClient _udpClient;
        private Thread? _listenThread;
        private CancellationTokenSource? _cancellationTokenSource;
        private Dictionary<string, IPEndPoint> _connectedClients = new Dictionary<string, IPEndPoint>();
        public int Port { get; private set; }
        public bool isServerRunning { get; private set; }

        public UDPServer(int port)
        {
            isServerRunning = false;
            Port = port;
            _udpClient = new UdpClient(Port);
        }

        public void StartInBackground()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _listenThread = new Thread(() => StartListening(_cancellationTokenSource.Token));
            _listenThread.Start();
        }

        public void StartListening(CancellationToken cancellationToken)
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                while (!cancellationToken.IsCancellationRequested)
                {
                    isServerRunning = true;
                    byte[] receivedBytes = _udpClient.Receive(ref remoteEndPoint);
                    string clientId = GenerateUniqueClientId(remoteEndPoint); // Generate a unique identifier
                    _connectedClients[clientId] = remoteEndPoint;

                    string data = Encoding.ASCII.GetString(receivedBytes);
                    ConsoleUtility.Print($"Received UDP data from client {clientId}: {data}");

                    // Implement your UDP server logic here...

                    // You can use the client ID to send a response, if needed
                    SendUdpData(clientId, "Response to client " + clientId);
                    ReceiveUdpData(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                ConsoleUtility.Print("UDP Error: " + ex.Message);
            }
        }

        public void Stop()
        {
            _udpClient?.Close();
            ConsoleUtility.Print("UDP Server has stopped!");
        }

        public void SendUdpData(string clientId, string message)
        {
            if (_connectedClients.TryGetValue(clientId, out IPEndPoint remoteEndPoint))
            {
                try
                {
                    byte[] sendData = Encoding.ASCII.GetBytes(message);
                    _udpClient.Send(sendData, sendData.Length, remoteEndPoint);
                    ConsoleUtility.Print("Sent UDP data to client " + clientId + ": " + message);
                }
                catch (Exception ex)
                {
                    ConsoleUtility.Print("UDP Error: " + ex.Message);
                }
            }
        }

        public List<Client> GetConnectedUdpClientIds()
        {
            //return new List<string>(_connectedClients.Keys);
            return _connectedClients.Select(client => new Client
            {
                Id = client.Key,
                IP = client.Value.Address.ToString(),
                Port = client.Value.Port
            }).ToList();
        }

        private void ReceiveUdpData(CancellationToken cancellationToken)
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                while (!cancellationToken.IsCancellationRequested)
                {
                    isServerRunning = true;
                    byte[] receivedBytes = _udpClient.Receive(ref remoteEndPoint);
                    string data = Encoding.ASCII.GetString(receivedBytes);
                    ConsoleUtility.Print("Received UDP data: " + data);

                    // Implement your UDP server logic here...
                }
            }
            catch (Exception ex)
            {
                ConsoleUtility.Print("UDP Error: " + ex.Message);
            }
        }

        private string GenerateUniqueClientId(IPEndPoint remoteEndPoint)
        {
            return $"{remoteEndPoint.Address}:{remoteEndPoint.Port}";
        }

        //private void ReceiveUdpDataAsync()
        //{
        //    try
        //    {
        //        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        //        while (true)
        //        {
        //            byte[] receivedBytes = await udpClient.ReceiveAsync();
        //            string data = Encoding.ASCII.GetString(receivedBytes);
        //            ConsoleUtility.Print("Received UDP data: " + data);

        //            // Implement your UDP server logic here...
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ConsoleUtility.Print("UDP Error: " + ex.Message);
        //    }
        //}
    }
}
