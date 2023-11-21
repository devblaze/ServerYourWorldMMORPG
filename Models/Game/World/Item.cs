using System.Numerics;

namespace ServerYourWorldMMORPG.Models.Game.World
{
	public class Item
	{
		public BigInteger Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public ItemType Type { get; set; }
	}
}
