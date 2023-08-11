using ServerYourWorldMMORPG.Models;
using ServerYourWorldMMORPG.GameServer;

namespace ServerYourWorldMMORPG.Services.Interfaces
{
    public interface INetworkServer
    {
        void Start();
        void Stop();
        bool IsServerRunning();
        void Initialize();
        List<Client> GetConnectedClients();
    }
}
