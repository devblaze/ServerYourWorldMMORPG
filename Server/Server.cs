using Org.BouncyCastle.Utilities.Net;
using ServerYourWorldMMORPG.Models.Tcp;
using ServerYourWorldMMORPG.Models.Udp;
using System.Net;

namespace ServerYourWorldMMORPG.Server
{
    public class Server : IServer
    {
        private TCPServer tcpServer;
        private UDPServer udpServer;

        public Server(ServerSettings settings)
        {
            //Console.WriteLine($"Inside Server: {settings.IpAddress} / {settings.TcpPort} / {settings.UdpPort} / {settings.MaxPlayers}");
            tcpServer = new TCPServer(settings.IpAddress, settings.TcpPort, settings.MaxPlayers);
            udpServer = new UDPServer(settings.UdpPort);

            // Initialize other server components or settings here

            // For example:
            // InitializeGameLogic();
        }

        public void Start()
        {
            tcpServer.Start();
            udpServer.Start();
        }

        public void Stop()
        {
            tcpServer?.Stop();
            udpServer?.Stop();
        }

        // Implement your game logic methods here
        private void HandleTcpData(string clientId, string data)
        {
            // Process TCP data and respond to the client
        }

        private void HandleUdpData(string data, IPEndPoint remoteEndPoint)
        {
            // Process UDP data and respond to the client
        }

        // Additional methods for initializing game logic, if needed
        private void InitializeGameLogic()
        {
            // Initialize game-related components and logic
        }
    }
}
