using ServerYourWorldMMORPG.Models.Constants;
using ServerYourWorldMMORPG.Models.Network;
using ServerYourWorldMMORPG.Services.Interfaces;
using ServerYourWorldMMORPG.Services.Network;
using ServerYourWorldMMORPG.Utils;
using System.Net;

namespace ServerYourWorldMMORPG.GameServer
{
    public class NetworkServer : INetworkServer
    {
        private TCPServer _tcpServer;
        private UDPServer _udpServer;
        private CancellationTokenSource _cancellationTokenSource;
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

        public void StartLoginServer(bool WithThread = false)
        {
            _tcpServer.StartListeningAsync();
        }

        public void StopLoginServer()
        {
            _cancellationTokenSource.Cancel();
            _tcpServer.Stop();
        }

        public void StopGameServer()
        {
            _udpServer.Stop();
        }

        public void ServerStatus(string[] arguments)
        {
            //var test = this.GetType().GetMethod("IsLoginServerRunning");
            //if (test != null)
            //{
            //    ConsoleUtility.Print("Test is working!");
            //}
            try
            {
                string gameServerStatus = IsGameServerRunning() ? "ONLINE" : "OFFLINE";
                string loginServerStatus = IsLoginServerRunning() ? "ONLINE" : "OFFLINE";

                if (arguments.Length > 0)
                {
                    string status = "";
                    if (arguments[0].Contains(CommandsWordings.GAMESERVER)) status = gameServerStatus;
                    if (arguments[0].Contains(CommandsWordings.LOGINSERVER)) status = loginServerStatus;

                    ConsoleUtility.Print(arguments[0] + " server is: " + status);
                }

                ConsoleUtility.Print(CommandsWordings.LOGINSERVER + " server is: " + loginServerStatus);
                ConsoleUtility.Print(CommandsWordings.GAMESERVER + " server is: " + gameServerStatus);
            }
            catch (Exception ex)
            {
                ConsoleUtility.Print("Error: " + ex.Message);
            }
        }

        public bool IsLoginServerRunning()
        {
            return _tcpServer.IsServerRunning;
        }

        public bool IsGameServerRunning()
        {
            return _udpServer.IsServerRunning;
        }

        public List<UserClient> GetConnectedClients()
        {
            List<UserClient> connectedClients = new List<UserClient>();
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

            //string[] ipAddressPort = data[0].Split(':');
            //if (ipAddressPort.Length != 2)
            //{
            //    ConsoleUtility.Print("Invalid IP:Port format.");
            //    return;
            //}

            //string ipAddress = ipAddressPort[0];
            //if (!IPAddress.TryParse(ipAddress, out IPAddress targetIPAddress))
            //{
            //    ConsoleUtility.Print("Invalid IP address.");
            //    return;
            //}

            //if (!int.TryParse(ipAddressPort[1], out int targetPort))
            //{
            //    ConsoleUtility.Print("Invalid port.");
            //    return;
            //}
            //IPEndPoint targetEndPoint = new IPEndPoint(targetIPAddress, targetPort);

            string clientId = data[0];
            string message = data[1];
            try
            {
                // Check if the client is connected before sending
                if (_udpServer.IsClientIdConnected(clientId))
                {
                    _udpServer.SendUdpData(clientId, message);
                    ConsoleUtility.Print($"Sent UDP data to client {clientId}: {message}");
                }
                else
                {
                    ConsoleUtility.Print($"Client {clientId} is not connected or listening.");
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
