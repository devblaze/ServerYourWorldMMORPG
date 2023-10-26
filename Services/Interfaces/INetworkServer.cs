using ServerYourWorldMMORPG.Models;

namespace ServerYourWorldMMORPG.Services.Interfaces
{
    public interface INetworkServer
    {
        void SendMockPacket(string[] data);
        void StartLoginServer();
        void StartGameServer();
        void StopLoginServer();
        void StopGameServer();
        void Initialize();
        bool IsLoginServerRunning();
        bool IsGameServerRunning();
        List<Client> GetConnectedClients();
    }
}
