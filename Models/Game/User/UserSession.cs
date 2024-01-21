using System.Net;
using System.Net.Sockets;

namespace ServerYourWorldMMORPG.Models.Game.User
{
	public class UserSession
	{
		public TcpClient TcpClient { get; set; }
		public IPEndPoint ClientIpEndPoint { get; set; }
		public Character Character { get; set; }
		public NetworkObject PlayerNetworkObject { get; set; }
		public DateTime LastMessageReceived { get; set; }
	}
}
