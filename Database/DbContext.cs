using Microsoft.EntityFrameworkCore;
using ServerYourWorldMMORPG.Models.Game.User;
using ServerYourWorldMMORPG.Models.Game.World;
using ServerYourWorldMMORPG.Models.Game.World.NPC;

namespace ServerYourWorldMMORPG.Database
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public DbSet<Account> Accounts { get; set; }
		public DbSet<Character> Characters { get; set; }
		public DbSet<Item> Items { get; set; }
		public DbSet<ItemType> ItemsType { get; set; }
		public DbSet<Monster> Monsters { get; set; }
		public DbSet<MonsterType> MonsterTypes { get; set; }
		public DbSet<Teritory> Territories { get; set; }
		public DbSet<Teritory> TerritoryTypes { get; set; }
		public DbSet<Inventory> Inventory { get; set; }
		public DbSet<NonPlayerCharacter> NPCs { get; set; }
		public DbSet<NonPlayerCharacterType> NPCTypes { get; set; }
		public DbSet<Guild> Guilds { get; set; }
	}
}
