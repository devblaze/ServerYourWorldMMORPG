using System.Numerics;

namespace ServerYourWorldMMORPG.Models.Game.World.NPC
{
	public class Monster
	{
		public string Name { get; set; }
		public int Level { get; set; }
		public MonsterType Type { get; set; }
		public Vector3 Position { get; set; }
	}
}
