using ServerYourWorldMMORPG.Models.Game;
using ServerYourWorldMMORPG.Services.Application.Interfaces;
using System.Numerics;
using System.Text.Json;

namespace ServerYourWorldMMORPG.Services.Application.GameServer
{
	public class NetworkObjectService
	{
		private IGameServerService _gameServerService;
		private Dictionary<string, NetworkObject> _networkObjects = new Dictionary<string, NetworkObject>();

		public NetworkObjectService(IGameServerService gameServerService)
		{
			_gameServerService = gameServerService;
		}

		public void RegisterNetworkObject(string id, Vector3 position, Quaternion rotation)
		{
			var networkObject = new NetworkObject
			{
				NetworkId = id,
				NetworkPosition = position,
				Rotation = rotation
			};
			_networkObjects[id] = networkObject;
		}

		public void UpdateNetworkObject(string id, Vector3 newPosition, Quaternion newRotation)
		{
			if (_networkObjects.TryGetValue(id, out var networkObject))
			{
				networkObject.NetworkPosition = newPosition;
				networkObject.Rotation = newRotation;
				BroadcastNetworkObjectUpdate(networkObject);
			}
		}

		private void BroadcastNetworkObjectUpdate(NetworkObject networkObject)
		{
			string message = CreateUpdateMessage(networkObject);
			// Assuming SendUdpData is a method in IGameServerService to send data to all clients
			//_gameServerService.SendUdpDataToAllClients(message);
		}

		private string CreateUpdateMessage(NetworkObject networkObject)
		{
			// Serialize the NetworkObject to JSON string
			return JsonSerializer.Serialize(networkObject);

		}
	}
}
