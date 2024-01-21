namespace ServerYourWorldMMORPG.Models.Game.User
{
	public class Player
	{
		public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

		public ushort Id { get; set; }
		public string Username { get; set; }

		public static void Spawn(ushort id, string username)
		{
			Player player = new Player
			{
				Id = id,
				Username = username
			};

			list.Add(id, player);
		}

		//public void RemovePlayer(ushort playerId)
		//{
		//	list.Remove(playerId);
		//}

		//[MessageHandler((ushort)ClientToServerId.name)]
		//private static void Name(ushort fromClientId, Message message)
		//{
		//	Spawn(fromClientId, message.GetString());
		//}
	}
}
