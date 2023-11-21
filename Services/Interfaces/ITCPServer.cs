using ServerYourWorldMMORPG.Models.Network;
using System.Net.Sockets;

namespace ServerYourWorldMMORPG.Services.Interfaces
{
    public interface ITCPServer
    {
        Task Stop();
        void StartListeningAsync();
        List<UserClient> GetConnectedTcpClients();
        Task DisconnectClients(Dictionary<string, TcpClient> clientsToDisconnect);
    }
}