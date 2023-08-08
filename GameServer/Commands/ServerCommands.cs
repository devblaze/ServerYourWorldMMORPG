using System;
using ServerYourWorldMMORPG.Utils;

namespace ServerYourWorldMMORPG.GameServer.Commands
{
    public class ServerCommands : IServerCommands
    {
        private bool _isServerRunning = false;
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

            ConsoleUtility.Print("Server is already running.");
            return;
        }

        public void StopServer()
        {
            if (_server.IsServerRunning())
            {
                ConsoleUtility.Print("Server stopped!");
                _server.Stop();
            }

            ConsoleUtility.Print("Server is not running.");
            return;
        }
    }
}
