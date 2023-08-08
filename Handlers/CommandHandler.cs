using ServerYourWorldMMORPG.MockClients;
using ServerYourWorldMMORPG.GameServer;
using ServerYourWorldMMORPG.GameServer.Commands;
using ServerYourWorldMMORPG.Utils;

namespace ServerYourWorldMMORPG.Handlers
{
    public class CommandHandler
    {
        private readonly IServerCommands _serverCommandService;
        private INetworkServer _server;

        public CommandHandler(INetworkServer server, IServerCommands serverCommandService)
        {
            _server = server;
            _serverCommandService = serverCommandService;
        }

        public void ProcessCommand(string? input)
        {
            if (input == null) return;

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
                // Handle more commands here
                default:
                    ConsoleUtility.Print("Unknown command.");
                    break;
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
                Console.WriteLine("Usage: sendmockpacket <data>");
            }
        }
    }
}
