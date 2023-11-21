using ServerYourWorldMMORPG.Models.Game.World;

namespace ServerYourWorldMMORPG.Models.Game.User
{
	public class Inventory
	{
		public Player Player { get; set; }
		public Item Item { get; set; }
		public int quantity { get; set; }
		public int quality { get; set; }
	}
}
