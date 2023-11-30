using ServerYourWorldMMORPG.Models.Application.Network;

namespace ServerYourWorldMMORPG.Services.Application.Interfaces
{
	public interface IGameServerService
	{
		bool ServerStatus();
		Task StartServer();
		Task StopServer();
		List<UserClient> GetConnectedClients();
	}
}
