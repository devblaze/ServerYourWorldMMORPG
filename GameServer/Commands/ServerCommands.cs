using ServerYourWorldMMORPG.Services.Interfaces;
using ServerYourWorldMMORPG.Utils;

namespace ServerYourWorldMMORPG.GameServer.Commands
{
    public class ServerCommands : IServerCommands
    {
        private INetworkServer _server;

        public ServerCommands(INetworkServer server)
        {
            _server = server;
        }

        public void StartServer()
        {
            if (!_server.IsServerRunning())
            {
                ConsoleUtility.Print("Server started.");
                _server.Start();
            }
            else
            {
                ConsoleUtility.Print("Server is already running.");
            }
        }

        public void StopServer()
        {
            if (_server.IsServerRunning())
            {
                ConsoleUtility.Print("Server stopped!");
                _server.Stop();
            }
            else
            {
                ConsoleUtility.Print("Server is not running.");
            }
        }
    }
}
