using Microsoft.EntityFrameworkCore;
using ServerYourWorldMMORPG.Models.Game.User;
using ServerYourWorldMMORPG.Utils;

namespace ServerYourWorldMMORPG.Database
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Character> Characters { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			ApplicationSettings.LoadSettings();
			optionsBuilder.UseMySQL(ApplicationSettings.ConnectionString);
		}
	}
}
