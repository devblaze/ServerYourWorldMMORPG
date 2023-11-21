using ServerYourWorldMMORPG.Models.Network;

namespace ServerYourWorldMMORPG.Services.Interfaces
{
    public interface INetworkServer
    {
        void Initialize();
        void StartGameServer();
        void StartLoginServer(bool WithThread = false);
        void StopLoginServer();
        void StopGameServer();
        void ServerStatus(string[] arguments);
        bool IsLoginServerRunning();
        bool IsGameServerRunning();
        List<UserClient> GetConnectedClients();
        void SendMockPacket(string[] data);
    }
}
