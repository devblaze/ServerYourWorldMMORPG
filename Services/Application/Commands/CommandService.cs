using Microsoft.EntityFrameworkCore;
using ServerYourWorldMMORPG.Database;
using ServerYourWorldMMORPG.Models.Constants;
using ServerYourWorldMMORPG.Services.Application.Interfaces;
using ServerYourWorldMMORPG.Utils;

namespace ServerYourWorldMMORPG.Services.Application.Commands
{
    public class CommandService : ICommandService, IDisposable
    {
        private readonly IServerCommands _serverCommandService;
        private readonly ApplicationDbContext _dbContext;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public CommandService(IServerCommands serverCommandService, ApplicationDbContext dbContext)
        {
            _serverCommandService = serverCommandService;
            _dbContext = dbContext;
        }

        public async Task Initialize()
        {
            //await DatabaseUpdate();

            Task.Run(() => _serverCommandService.ExecuteCommand(CommandsWordings.START, new string[0]),
                _cancellationTokenSource.Token);

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    ConsoleUtility.Print("Enter your command: ");
                    var input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input)) continue;

                    var commandParts = input.Split(' ');
                    var command = commandParts[0].ToLower();
                    var arguments = commandParts.Skip(1).ToArray();

                    Task.Run(() => _serverCommandService.ExecuteCommand(command, arguments),
                        _cancellationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    ConsoleUtility.Print("An error occurred: " + ex.Message);
                }
            }
        }

        public async Task DatabaseUpdate()
        {
            try
            {
                ConsoleUtility.Print("Migrating database...");
                await _dbContext.Database.MigrateAsync();
                ConsoleUtility.Print("Database migration completed.");
            }
            catch (Exception ex)
            {
                ConsoleUtility.Print($"Error during database migration: {ex.Message}");
                // Consider whether to continue or stop the application
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }
    }
}
