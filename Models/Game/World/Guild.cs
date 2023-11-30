using ServerYourWorldMMORPG.Models.Game.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerYourWorldMMORPG.Models.Game.World
{
	public class Guild
	{
		public Guid Id { get; set; }
		[ForeignKey("OwnerId")]
		public Character character { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Level { get; set; }
	}
}
