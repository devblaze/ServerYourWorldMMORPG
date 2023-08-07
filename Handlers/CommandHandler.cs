using ServerYourWorldMMORPG.MockClients;
using ServerYourWorldMMORPG.Models.Utils;

namespace ServerYourWorldMMORPG.Server
{
    public class CommandHandler
    {
        private readonly ServerCommands _serverCommands;
        private IServer server;

        public CommandHandler(IServer server)
        {
            this.server = server;
            _serverCommands = new ServerCommands();
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
                    this.server.Start();
                    break;
                case "stop":
                    this.server.Stop();
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
