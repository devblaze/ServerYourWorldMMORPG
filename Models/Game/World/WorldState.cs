using ServerYourWorldMMORPG.Models.Game.User;
using ServerYourWorldMMORPG.Models.Game.World.NPC;

namespace ServerYourWorldMMORPG.Models.Game.World
{
	public class WorldState
	{
		public List<Character> Players { get; set; }
		public List<Monster> Monsters { get; set; }
		public Dictionary<Guid, Teritory> Teritories { get; set; }
	}
}
