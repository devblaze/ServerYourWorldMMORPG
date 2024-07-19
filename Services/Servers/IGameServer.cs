namespace ServerYourWorldMMORPG.Services.Application.GameServer
{
    public interface IGameServer
    {
        Task StartServer();
        Task StopServer();
    }
}