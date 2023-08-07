using System;

namespace ServerYourWorldMMORPG.Server
{
    public class ServerCommands
    {
        private bool _isServerRunning = false;

        public void StartServer()
        {
            if (!_isServerRunning)
            {
                // Start server logic
                Console.WriteLine("Server started.");
                _isServerRunning = true;
            }
            else
            {
                Console.WriteLine("Server is already running.");
            }
        }

        public void StopServer()
        {
            if (_isServerRunning)
            {
                // Stop server logic
                Console.WriteLine("Server stopped.");
                _isServerRunning = false;
            }
            else
            {
                Console.WriteLine("Server is not running.");
            }
        }
    }
}
