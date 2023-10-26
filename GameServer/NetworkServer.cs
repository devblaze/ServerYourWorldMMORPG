using ServerYourWorldMMORPG.Models;
using ServerYourWorldMMORPG.Services.Interfaces;
using ServerYourWorldMMORPG.Utils;
using System.Net;

namespace ServerYourWorldMMORPG.GameServer
{
    public class NetworkServer : INetworkServer
    {
        private TCPServer _tcpServer;
        private UDPServer _udpServer;
        private Dictionary<string, bool> _connectedClients = new Dictionary<string, bool>();

        public NetworkServer()
        {
            Initialize();
        }

        public void Initialize()
        {
            ServerSettings.LoadSettings();
            _tcpServer = new TCPServer(ServerSettings.IpAddress, ServerSettings.TcpPort, ServerSettings.MaxPlayers);
            _udpServer = new UDPServer(ServerSettings.UdpPort);
        }

        public void StartGameServer()
        {
            _udpServer.StartInBackground();
        }

        public void StartLoginServer()
        {
            _tcpServer.StartInBackground();
        }

        public void StopLoginServer()
        {
            _tcpServer.Stop();
        }

        public void StopGameServer()
        {
            _udpServer.Stop();
        }

        public bool IsLoginServerRunning()
        {
            return _tcpServer.isServerRunning;
        }

        public bool IsGameServerRunning()
        {
            return _udpServer.isServerRunning;
        }

        public List<Client> GetConnectedClients()
        {
            List<Client> connectedClients = new List<Client>();
            connectedClients.AddRange(_tcpServer.GetConnectedTcpClients());
            connectedClients.AddRange(_udpServer.GetConnectedUdpClientIds());
            return connectedClients;
        }

        public void SendMockPacket(string[] data)
        {
            if (data.Length < 2)
            {
                ConsoleUtility.Print("Usage: sendmockpacket <ip:port> <message>");
                return;
            }

            string[] ipAddressPort = data[0].Split(':');
            if (ipAddressPort.Length != 2)
            {
                ConsoleUtility.Print("Invalid IP:Port format.");
                return;
            }

            string ipAddress = ipAddressPort[0];
            if (!IPAddress.TryParse(ipAddress, out IPAddress targetIPAddress))
            {
                ConsoleUtility.Print("Invalid IP address.");
                return;
            }

            if (!int.TryParse(ipAddressPort[1], out int targetPort))
            {
                ConsoleUtility.Print("Invalid port.");
                return;
            }

            string message = data[1];
            IPEndPoint targetEndPoint = new IPEndPoint(targetIPAddress, targetPort);

            try
            {
                // Check if the client is connected before sending
                if (IsClientConnected(targetEndPoint))
                {
                    _udpServer.SendUdpData(message, targetEndPoint);
                    ConsoleUtility.Print($"Sent UDP data to {targetEndPoint}: {message}");
                }
                else
                {
                    ConsoleUtility.Print($"Client {targetEndPoint} is not connected or listening.");
                }
            }
            catch (Exception ex)
            {
                ConsoleUtility.Print("UDP Error: " + ex.Message);
            }
        }

        // Add a client to the connected clients dictionary when a client connects
        private void AddConnectedClient(IPEndPoint clientEndPoint)
        {
            string key = clientEndPoint.ToString();
            _connectedClients[key] = true;
        }

        // Remove a client from the connected clients dictionary when a client disconnects
        private void RemoveConnectedClient(IPEndPoint clientEndPoint)
        {
            string key = clientEndPoint.ToString();
            _connectedClients.Remove(key);
        }

        // Check if a client is connected
        private bool IsClientConnected(IPEndPoint clientEndPoint)
        {
            string key = clientEndPoint.ToString();
            return _connectedClients.ContainsKey(key) && _connectedClients[key];
        }
    }
}
