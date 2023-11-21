using ServerYourWorldMMORPG.Services.Interfaces;
using ServerYourWorldMMORPG.Utils;

namespace ServerYourWorldMMORPG.Services
{
    public class CommandService : ICommandService
    {
        private readonly IServerCommands _serverCommandService;
        private CancellationTokenSource _cancellationTokenSource;

        public CommandService(IServerCommands serverCommandService, CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _serverCommandService = serverCommandService;
        }

        public async Task Initialize()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                ConsoleUtility.Print("Enter your command: ");
                string input = Console.ReadLine();
                string[] commandParts = input.Split(' ');
                string command = commandParts[0].ToLower();
                string[] arguments = commandParts.Skip(1).ToArray();

                await Task.Run(() => _serverCommandService.ExecuteCommand(command, arguments),
                    _cancellationTokenSource.Token);
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
