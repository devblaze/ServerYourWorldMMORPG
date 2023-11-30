using System.Numerics;

namespace ServerYourWorldMMORPG.Models.Game
{
	public class NetworkObject
	{
		public string NetworkId { get; set; }
		public Vector3 NetworkPosition { get; set; }
		public Quaternion Rotation { get; set; }
	}
}
