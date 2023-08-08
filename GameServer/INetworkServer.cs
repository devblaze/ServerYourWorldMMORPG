namespace ServerYourWorldMMORPG.GameServer
{
    public interface INetworkServer
    {
        void Start();
        void Stop();
        bool IsServerRunning();
        void Initialize(ServerSettings serverSettings);
    }
}
