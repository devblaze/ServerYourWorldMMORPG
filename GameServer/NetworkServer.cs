using ServerYourWorldMMORPG.Models;

namespace ServerYourWorldMMORPG.GameServer
{
    public class NetworkServer : INetworkServer
    {
        private TCPServer tcpServer;
        private UDPServer udpServer;

        public NetworkServer(ServerSettings settings)
        {
            tcpServer = new TCPServer(settings.IpAddress, settings.TcpPort, settings.MaxPlayers);
            udpServer = new UDPServer(settings.UdpPort);

        }

        public void Initialize(ServerSettings settings)
        {
            tcpServer = new TCPServer(settings.IpAddress, settings.TcpPort, settings.MaxPlayers);
            udpServer = new UDPServer(settings.UdpPort);
        }

        public void Start()
        {
            tcpServer.StartInBackground();
            udpServer.StartInBackground();
        }

        public void Stop()
        {
            tcpServer?.Stop();
            udpServer?.Stop();
        }

        public bool IsServerRunning()
        {
            return tcpServer.isServerRunning && udpServer.isServerRunning;
        }
    }
}
