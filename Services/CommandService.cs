using ServerYourWorldMMORPG.Services.Interfaces;
using ServerYourWorldMMORPG.Utils;

namespace ServerYourWorldMMORPG.Services
{
    public class CommandService : ICommandService
    {
        private readonly IServerCommands _serverCommandService;
        private INetworkServer _server;
        private CancellationTokenSource _cancellationTokenSource;

        public CommandService(INetworkServer server,
            IServerCommands serverCommandService,
            CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _server = server;
            _serverCommandService = serverCommandService;
        }

        public async Task ProcessCommandAsync(string input)
        {
            string[] commandParts = input.Split(' ');
            string command = commandParts[0].ToLower();
            string[] arguments = commandParts.Skip(1).ToArray();

            await _serverCommandService.ExecuteCommand(command, arguments);
        }

        public async Task InitializeAsync()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                ConsoleUtility.Print("Command: ");
                string input = Console.ReadLine();
                await ProcessCommandAsync(input);
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
