using ServerYourWorldMMORPG.Models;
using ServerYourWorldMMORPG.Models.Constants;
using ServerYourWorldMMORPG.Services.Interfaces;
using ServerYourWorldMMORPG.Utils;
using System.Diagnostics;

namespace ServerYourWorldMMORPG.GameServer.Commands
{
    public class ServerCommands : IServerCommands
    {
        private INetworkServer _server;

        public ServerCommands(INetworkServer server)
        {
            _server = server;
        }

        public async Task ExecuteCommand(string command, string[] arguments)
        {
            switch (command)
            {
                case CommandsWordings.START:
                    StartServer(arguments);
                    break;
                case CommandsWordings.STOP:
                    StopServer(arguments);
                    break;
                case CommandsWordings.MOCKPACKET:
                    ProcessSendMockPacket(arguments);
                    break;
                case CommandsWordings.CLIENTS:
                    DisplayConnectedClients();
                    break;
                case CommandsWordings.FAKECLIENT:
                    StartDummyGameClient();
                    break;
                default:
                    ConsoleUtility.Print("Unknown command.");
                    break;
            }
        }

        public void StartServer(string[] arguments)
        {
            if (arguments.Length > 0)
            {
                StartSpecificServer(arguments[0]);
                return;
            }

            StartSpecificServer(CommandsWordings.LOGINSERVER);
            StartSpecificServer(CommandsWordings.GAMESERVER);
        }

        public void StopServer(string[] arguments)
        {
            if (arguments.Length > 0)
            {
                StopSpecificServer(arguments[0]);
                return;
            }

            StopSpecificServer(CommandsWordings.LOGINSERVER);
            StopSpecificServer(CommandsWordings.GAMESERVER);
        }

        public void DisplayConnectedClients()
        {
            List<Client> connectedClients = _server.GetConnectedClients();

            if (connectedClients.Count <= 0)
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

        public void ProcessSendMockPacket(string[] arguments)
        {
            if (arguments.Length < 2)
            {
                ConsoleUtility.Print("Usage: mockpacket <ip:port> <message>");
                return;
            }

            _server.SendMockPacket(arguments);
        }

        private void StartDummyGameClient()
        {
            // Specify the relative path to your DummyGameClientApp.exe
            string dummyClientAppPath = @"..\DummyGameClientApp\bin\Debug\DummyGameClientApp.exe";

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = dummyClientAppPath,
                    CreateNoWindow = false, // Show the console window
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();
                }

                ConsoleUtility.Print("Started the Dummy Game Client.");
            }
            catch (Exception ex)
            {
                ConsoleUtility.Print("Error starting the Dummy Game Client: " + ex.Message);
            }
        }

        private void StopSpecificServer(string serverToStart)
        {
            if (serverToStart == CommandsWordings.GAMESERVER)
            {
                if (_server.IsGameServerRunning())
                {
                    _server.StopGameServer();
                    ConsoleUtility.Print("Game server has stopped!");
                }
                else
                {
                    ConsoleUtility.Print("Game server is not running.");
                }
            }

            if (serverToStart == CommandsWordings.LOGINSERVER)
            {
                if (_server.IsLoginServerRunning())
                {
                    _server.StopLoginServer();
                    ConsoleUtility.Print("Login server has stopped!");
                }
                else
                {
                    ConsoleUtility.Print("Login server is not running.");
                }
            }
        }

        private void StartSpecificServer(string serverToStart)
        {
            if (serverToStart == CommandsWordings.GAMESERVER)
            {
                if (!_server.IsGameServerRunning())
                {
                    _server.StartGameServer();
                    ConsoleUtility.Print("Game server started!");
                }
                else
                {
                    ConsoleUtility.Print("Game server is already running!");
                }
            }

            if (serverToStart == CommandsWordings.LOGINSERVER)
            {
                if (!_server.IsLoginServerRunning())
                {
                    _server.StartLoginServer();
                    ConsoleUtility.Print("Login server started!");
                }
                else
                {
                    ConsoleUtility.Print("Login server is already running!");
                }
            }
        }
    }
}
