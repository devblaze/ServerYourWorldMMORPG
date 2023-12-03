using ServerYourWorldMMORPG.Models.Application.Network;
using System.Net.Sockets;

namespace ServerYourWorldMMORPG.Services.Application.Interfaces
{
	public interface IGameServerService
	{
		bool ServerStatus();
		Task StartServer();
		Task StopServer();
		List<UserClient> GetConnectedClients();
		Task SendMessage(string clientId, string message, ProtocolType protocol = ProtocolType.Udp);
		Task BroadcastMessage(string message);
	}
}
