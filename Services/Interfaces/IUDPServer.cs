using ServerYourWorldMMORPG.Models.Network;

namespace ServerYourWorldMMORPG.Services.Interfaces
{
    public interface IUDPServer
    {
        void StartInBackground();
        void StartListening(CancellationToken cancellationToken);
        void Stop();
        void SendUdpData(string clientId, string message);
        List<UserClient> GetConnectedUdpClientIds();
        bool IsClientIdConnected(string clientId);
    }
}