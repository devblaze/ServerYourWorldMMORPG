using System.Numerics;

namespace ServerYourWorldMMORPG.Models.Game.User
{
	public class Player
	{
		public Guid Id { get; set; }
		public string Username { get; set; }
		public int Level { get; set; }
		public float Experience { get; set; }
		public float Health { get; set; }
		public float Mana { get; set; }
		public Vector3 Position { get; set; }
	}
}
