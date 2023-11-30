using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerYourWorldMMORPG.Database;
using ServerYourWorldMMORPG.Models.Application;
using ServerYourWorldMMORPG.Models.Application.Network;
using ServerYourWorldMMORPG.Services.Application.Commands;
using ServerYourWorldMMORPG.Services.Application.GameServer;
using ServerYourWorldMMORPG.Services.Application.Interfaces;
using ServerYourWorldMMORPG.Services.Application.LoginServer;
using ServerYourWorldMMORPG.Services.Game;

public static class DependencyInjection
{
	public static IServiceProvider BuildServiceProvider()
	{
		var services = new ServiceCollection();

		var configuration = new ConfigurationBuilder()
			.SetBasePath("C:\\Users\\nikos\\Documents\\Visual Studio Projects\\ServerYourWorldMMORPG")
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build();

		var databaseSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();

		services.Configure<LoginServerSettings>(configuration.GetSection("LoginServerSettings"))
			.Configure<GameServerSettings>(configuration.GetSection("GameServerSettings"))
			.AddDbContext<ApplicationDbContext>(options => options.UseMySQL(databaseSettings.ConnectionString))
			.AddSingleton<ICommandService, CommandService>()
			.AddSingleton<IServerCommands, ServerCommands>()
			.AddSingleton<CancellationTokenSource>()
			.AddSingleton<ILoginServerService, LoginServerService>()
			.AddSingleton<IGameServerService, GameServerService>()
			.AddScoped<CharacterService>();

		return services.BuildServiceProvider();
	}
}
