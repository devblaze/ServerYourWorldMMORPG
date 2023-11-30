using ServerYourWorldMMORPG.Models.Application.Network;

namespace ServerYourWorldMMORPG.Services.Application.Interfaces
{
	public interface ILoginServerService
	{
		Task StartServer();
		Task StopServer();
		bool ServerStatus();
		void PrintAvailableGameServers();
		void RegisterGameServer(GameServerSettings serverInfo);
	}
}