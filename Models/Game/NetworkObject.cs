using System.Numerics;

namespace ServerYourWorldMMORPG.Models.Game
{
	[Serializable]
	public class NetworkObject
	{
		public string NetworkObjectId { get; set; }
		public Vector3 NetworkPosition { get; set; }
		public Quaternion NetworkRotation { get; set; }
	}
}
