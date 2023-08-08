using Microsoft.EntityFrameworkCore;
using ServerYourWorldMMORPG.Database;
using ServerYourWorldMMORPG.GameServer;
using ServerYourWorldMMORPG.Handlers;
using Microsoft.Extensions.DependencyInjection;
using ServerYourWorldMMORPG.Utils;

namespace ServerYourWorldMMORPG
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseConfirm();
            var serviceProvider = DependencyInjection.BuildServiceProvider();

            var networkServer = serviceProvider.GetRequiredService<INetworkServer>();
            var settings = ServerSettings.LoadSettings();
            networkServer.Initialize(settings);

            var commandHandler = serviceProvider.GetRequiredService<CommandHandler>();

            CommandLoop(commandHandler);
        }

        static void ConfigureShutdown(INetworkServer server)
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                ConsoleUtility.Print("Server shutting down...");
                server.Stop();
                ConsoleUtility.Print("Server shutdown complete.");
            };
        }

        private static void DatabaseConfirm()
        {
            Console.Title = "YourWorld Game Server";
            ConsoleUtility.Print("Migrating database...");
            using var dbContext = new ApplicationDbContext();
            dbContext.Database.Migrate();
        }

        private static void CommandLoop(CommandHandler commandHandler)
        {
            while (true)
            {
                //ConsoleUtility.Print("Command:");
                string? input = Console.ReadLine();
                commandHandler.ProcessCommand(input); // Process user commands using CommandHandler
            }
        }
    }
}
//var serviceProvider = new ServiceCollection()
//    .AddSingleton<INetworkServer>(new NetworkServer(settings))
//    .AddSingleton<CommandHandler>()
//    .BuildServiceProvider();

//var commandHandler = serviceProvider.GetRequiredService<CommandHandler>();

//var serviceProvider = DependencyInjection.BuildServiceProvider();
// Use the method from DependencyInjection

//var commandHandler = serviceProvider.GetRequiredService<CommandHandler>();

//var host = Host.CreateDefaultBuilder()
//    .ConfigureServices((hostContext, services) =>
//    {
//        services.AddSingleton<IGameServer, GameServer>();
//    }).Build();
//host.Run();