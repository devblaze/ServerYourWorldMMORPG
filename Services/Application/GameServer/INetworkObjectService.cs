using ServerYourWorldMMORPG.Models.Game;

namespace ServerYourWorldMMORPG.Services.Application.GameServer
{
	public interface INetworkObjectService
	{
		NetworkObject InitializeNetworkObject();
		string UpdateNetworkObjectWithMessage(string message);
		string NetworkObjectsListToJsonMessage();
		Task RemoveNetworkObject(NetworkObject networkObject);
	}
}