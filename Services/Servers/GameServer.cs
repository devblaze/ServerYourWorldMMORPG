using Microsoft.Extensions.Options;
using ServerYourWorldMMORPG.Models.Application.Network;
using ServerYourWorldMMORPG.Models.Game.User;
using ServerYourWorldMMORPG.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerYourWorldMMORPG.Services.Application.GameServer
{
    public class GameServer : IGameServer
    {
        private UdpClient _udpListener;
        private string name;
        private Dictionary<string, UserSession> _connectedUserSessions;
        private bool isRunning;
        private GameServerSettings _gameServerSettings;
        private INetworkObjectService _networkObjectService;
        private CancellationTokenSource _cancellationTokenSource;

        public GameServer(IOptions<GameServerSettings> gameServerSettings,
            INetworkObjectService networkObjectService)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _gameServerSettings = gameServerSettings.Value;
            name = _gameServerSettings.Name;
            isRunning = false;

            IPEndPoint udpEndPoint = new IPEndPoint(IPAddress.Parse(_gameServerSettings.IpAddress), _gameServerSettings.Port);
            _udpListener = new UdpClient(udpEndPoint);
            _connectedUserSessions = new Dictionary<string, UserSession>();

            _networkObjectService = networkObjectService;
        }

        public bool ServerStatus() => isRunning;

        public async Task StartServer()
        {
            isRunning = true;
            ConsoleUtility.Print($"{name} started.");
            await StartListening();

            while (isRunning)
            {
                // Main server loop for UDP listening and processing
                // TODO: implement RUDP features here
            }
        }

        public async Task StopServer()
        {
            isRunning = false;
            _cancellationTokenSource.Cancel();
            _udpListener.Close();
            _udpListener?.Dispose();
            _connectedUserSessions.Clear();
            ConsoleUtility.Print($"{name} stopped.");
        }

        private async Task StartListening()
        {
            using (var udpClient = new UdpClient(_gameServerSettings.Port))
            {
                var from = new IPEndPoint(0, 0);
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var receiveResult = await udpClient.ReceiveAsync();
                    Console.WriteLine($"Received a packet from {receiveResult.RemoteEndPoint}");

                    // Process the packet (pseudo-code)
                    // Check for ACKs, order, and duplicates

                    // Send an ACK back
                    var ackPacket = Encoding.UTF8.GetBytes("ACK");
                    await udpClient.SendAsync(ackPacket, ackPacket.Length, receiveResult.RemoteEndPoint);
                }
            }
        }

        //private void ProcessReceivedPacket(RudpPacket packet)
        //{
        //    // Decode the packet based on your game's protocol
        //    var message = Encoding.UTF8.GetString(packet.Data);

        //    ConsoleUtility.Print($"Received RUDP message: {message}");

        //    // Further processing based on the packet's content
        //    // For example, updating player positions, handling chat messages, etc.
        //}

        //private void HandleLostPacket(RudpPacket packet)
        //{
        //    // Handle lost packet scenarios, if applicable
        //    // This might involve logging, retrying, or other game-specific error handling
        //    ConsoleUtility.Print($"Packet lost: {packet.PacketId}");
        //}
    }
}
