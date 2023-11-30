namespace ServerYourWorldMMORPG.Models.Application.Network
{
	public class GameServerSettings
	{
		public string Name { get; set; }
		public string IpAddress { get; set; }
		public int Port { get; set; }
		public int CurrentPlayerCount { get; set; }
		public int MaxPlayers { get; set; }
	}
}
