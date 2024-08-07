﻿using ServerYourWorldMMORPG.Models.Game.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerYourWorldMMORPG.Models.Game.World
{
	public class Teritory
	{
		public int Id { get; set; }
		public Guid OwnerId { get; set; }
		[ForeignKey("OwnerId")]
		public Character character { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
