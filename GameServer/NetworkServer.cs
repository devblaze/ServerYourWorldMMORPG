using ServerYourWorldMMORPG.Models;
using ServerYourWorldMMORPG.Services.Interfaces;

namespace ServerYourWorldMMORPG.GameServer
{
    public class NetworkServer : INetworkServer
    {
        private TCPServer _tcpServer;
        private UDPServer _udpServer;

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

        public void Start()
        {
            _tcpServer.StartInBackground();
            _udpServer.StartInBackground();
        }

        public void Stop()
        {
            _tcpServer.Stop();
            _udpServer.Stop();
        }

        public bool IsServerRunning()
        {
            return _tcpServer.isServerRunning && _udpServer.isServerRunning;
        }

        public List<Client> GetConnectedClients()
        {
            List<Client> connectedClients = new List<Client>();
            connectedClients.AddRange(_tcpServer.GetConnectedClients());
            //connectedClients.AddRange(_udpServer.GetConnectedClients());
            return connectedClients;
        }
    }
}
