using Microsoft.EntityFrameworkCore;
using ServerYourWorldMMORPG.Models;
using ServerYourWorldMMORPG.Models.Game;

namespace ServerYourWorldMMORPG.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Character> Characters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=192.168.4.253;database=yourworld;user=root;password=Blaze23104289");
        }
    }
}
