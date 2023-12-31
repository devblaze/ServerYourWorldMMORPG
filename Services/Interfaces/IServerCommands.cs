﻿namespace ServerYourWorldMMORPG.Services.Interfaces
{
    public interface IServerCommands
    {
        Task ExecuteCommand(string command, string[] arguments);
        void StartServer(string[] arguments);
        void StopServer(string[] arguments);

        void DisplayConnectedClients();

        void ProcessSendMockPacket(string[] arguments);
    }
}