using System.Numerics;

namespace ServerYourWorldMMORPG.Models.Game.User
{
	public class Character
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Level { get; set; }
		public float Experience { get; set; }
		public float Health { get; set; }
		public float Mana { get; set; }
		public Vector3 Position { get; set; }
	}
}
