using Microsoft.EntityFrameworkCore;
using ServerYourWorldMMORPG.Database;
using ServerYourWorldMMORPG.Models.Utils;

namespace YourServerNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "YourWorld Game Server";
            ConsoleUtility.Print("Migrating database...");
            using var dbContext = new ApplicationDbContext();
            dbContext.Database.Migrate();

            // Instantiate your server class and start the server
            TCPServer server = new TCPServer();
            server.Start();
        }
    }
}
