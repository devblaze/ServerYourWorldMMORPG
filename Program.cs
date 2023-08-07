using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServerYourWorldMMORPG.Database;
using ServerYourWorldMMORPG.MockClients;
using ServerYourWorldMMORPG.Models.Utils;
using ServerYourWorldMMORPG.Server;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Text.Json.Nodes;

namespace YourServerNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ServerSettings.LoadSettings();
            DatabaseConfirm();

            IServer server = new Server(settings);
            CommandHandler commandHandler = new CommandHandler(server);

            var mockClient = new MockTcpClient();
            mockClient.StartInNewTerminal();

            ConfigureShutdown(server);
            //server.Start();
            CommandLoop(commandHandler);
        }

        static void ConfigureShutdown(IServer server)
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
                Console.Write("Enter a command: ");
                string? input = Console.ReadLine();
                commandHandler.ProcessCommand(input); // Process user commands using CommandHandler
            }
        }
    }
}
