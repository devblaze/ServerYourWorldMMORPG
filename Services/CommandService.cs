using ServerYourWorldMMORPG.Mocks;
using ServerYourWorldMMORPG.Models;
using ServerYourWorldMMORPG.Utils;
using ServerYourWorldMMORPG.Services.Interfaces;
using System.Threading;
using System.CommandLine.Parsing;

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

            switch (command)
            {
                case "start":
                    _serverCommandService.StartServer();
                    break;
                case "stop":
                    _serverCommandService.StopServer();
                    break;
                case "sendmockpacket":
                    ProcessSendMockPacket(arguments);
                    break;
                case "clients":
                    DisplayConnectedClients();
                    break;
                // Handle more commands here
                default:
                    ConsoleUtility.Print("Unknown command.");
                    break;
            }
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

        private void DisplayConnectedClients()
        {
            List<Client> connectedClients = _server.GetConnectedClients();

            if (connectedClients.Count == 0)
            {
                ConsoleUtility.Print("No clients connected.");
            }
            else
            {
                ConsoleUtility.Print("Connected Clients: ");
                foreach (Client client in connectedClients)
                {
                    Console.WriteLine($"Client ID: {client.Id} | Client IP: {client.IP} | Port: {client.Port}");
                }
            }
        }

        private void ProcessSendMockPacket(string[] arguments)
        {
            if (arguments.Length >= 1)
            {
                string data = string.Join(" ", arguments);
                //_serverCommands.SendMockPacket(data);
            }
            else
            {
                ConsoleUtility.Print("Usage: sendmockpacket <data>");
            }
        }
    }
}
