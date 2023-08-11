using Microsoft.EntityFrameworkCore;
using ServerYourWorldMMORPG.Database;
using Microsoft.Extensions.DependencyInjection;
using ServerYourWorldMMORPG.Utils;
using ServerYourWorldMMORPG.Services.Interfaces;

namespace ServerYourWorldMMORPG
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DatabaseConfirm();
            var serviceProvider = DependencyInjection.BuildServiceProvider();

            var networkServer = serviceProvider.GetRequiredService<INetworkServer>();

            var commandService = serviceProvider.GetRequiredService<ICommandService>();
            await commandService.InitializeAsync();
        }

        private static void DatabaseConfirm()
        {
            Console.Title = "YourWorld Game Server";
            ConsoleUtility.Print("Migrating database...");
            using var dbContext = new ApplicationDbContext();
            dbContext.Database.Migrate();
            ConsoleUtility.Print("Migrating database finished.");
        }
    }
}
