using System.Net;
using System.Net.Sockets;

namespace ServerYourWorldMMORPG.Models.Game.User
{
	public class UserSession
	{
		public TcpClient TcpClient { get; set; }
		public IPEndPoint UdpEndPoint { get; set; }
		public Character PlayerCharacter { get; set; }
		public NetworkObject PlayerNetworkObject { get; set; }
	}
}
