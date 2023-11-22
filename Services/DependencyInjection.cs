using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerYourWorldMMORPG.Database;
using ServerYourWorldMMORPG.GameServer;
using ServerYourWorldMMORPG.Services;
using ServerYourWorldMMORPG.Services.Game;
using ServerYourWorldMMORPG.Services.Interfaces;

public static class DependencyInjection
{
	public static IServiceProvider BuildServiceProvider()
	{
		var services = new ServiceCollection();

		var configuration = new ConfigurationBuilder();

		//configuration.SetBasePath(Directory.GetCurrentDirectory())
		//	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
		//	.Build();

		//var databaseSettings = configuration.GetSection();

		services.AddDbContext<ApplicationDbContext>()
			.AddSingleton<INetworkServer, NetworkServer>()
			.AddSingleton<ICommandService, CommandService>()
			.AddSingleton<CancellationTokenSource>()
			.AddSingleton<IDummyGameClient, DummyGameClient>()
			.AddScoped<PlayerService>()
			.BuildServiceProvider();

		return services.BuildServiceProvider();
	}
}
