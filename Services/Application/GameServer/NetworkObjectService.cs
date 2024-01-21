using Newtonsoft.Json;
using ServerYourWorldMMORPG.Models.Game;
using ServerYourWorldMMORPG.Utils;
using System.Numerics;

namespace ServerYourWorldMMORPG.Services.Application.GameServer
{
	public class NetworkObjectService : INetworkObjectService
	{
		private List<NetworkObject> _networkObjects = new List<NetworkObject>();

		public NetworkObject InitializeNetworkObject()
		{
			NetworkObject registeredNetworkObject = RegisterNetworkObject(GenerateUniqueNetworkObjectId());
			return registeredNetworkObject;
		}

		public string UpdateNetworkObjectWithMessage(string message)
		{
			var updatedNetworkObject = DeserializeMessage(message);
			if (updatedNetworkObject == null) return "";
			UpdateNetworkObject(updatedNetworkObject);

			return NetworkObjectsListToJsonMessage();
		}

		public string NetworkObjectsListToJsonMessage()
		{
			return JsonConvert.SerializeObject(_networkObjects);
		}

		public async Task RemoveNetworkObject(NetworkObject networkObject)
		{
			// How to remove the NetworkObject with it's ID
			// Accepts: string networkObjectIdToRemove
			//_networkObjects = _networkObjects.Where(networkObject => networkObject.NetworkObjectId != networkObjectIdToRemove).ToList();
			_networkObjects.Remove(networkObject);
		}

		private NetworkObject UpdateNetworkObject(NetworkObject networkObjectToUpdate)
		{
			var existingNetworkObject = FindNetworkObjectById(networkObjectToUpdate.NetworkObjectId);
			if (existingNetworkObject == null) return null;

			existingNetworkObject.NetworkPosition = networkObjectToUpdate.NetworkPosition;
			existingNetworkObject.NetworkRotation = networkObjectToUpdate.NetworkRotation;
			return existingNetworkObject;
		}

		private NetworkObject RegisterNetworkObject(string id)
		{
			var networkObject = new NetworkObject
			{
				NetworkObjectId = id,
				NetworkPosition = new Vector3(-1140, 1, -681),
				NetworkRotation = new Quaternion(0, 0, 0, 0)
			};
			_networkObjects.Add(networkObject);
			return networkObject;
		}

		private NetworkObject? DeserializeMessage(string message)
		{
			try
			{
				return JsonConvert.DeserializeObject<NetworkObject>(message);
			}
			catch (Exception ex)
			{
				ConsoleUtility.Print($"Error with JSON: {ex.Message}");
				return null;
			}
		}

		private string GenerateUniqueNetworkObjectId()
		{
			return Guid.NewGuid().ToString();
		}

		private NetworkObject FindNetworkObjectById(string id)
		{
			return _networkObjects.FirstOrDefault(obj => obj.NetworkObjectId == id);
		}
	}
}
