using Microsoft.Extensions.DependencyInjection;
using ServerYourWorldMMORPG.GameServer;
using ServerYourWorldMMORPG.GameServer.Commands;
using ServerYourWorldMMORPG.Handlers;

public static class DependencyInjection
{
    public static IServiceProvider BuildServiceProvider()
    {
        return new ServiceCollection()
            .AddSingleton<INetworkServer, NetworkServer>()
            .AddSingleton<IServerCommands, ServerCommands>()
            .AddSingleton<ServerSettings>()
            .AddSingleton<CommandHandler>()
            .BuildServiceProvider();
    }
}
