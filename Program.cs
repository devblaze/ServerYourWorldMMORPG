using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServerYourWorldMMORPG.Database;
using ServerYourWorldMMORPG.Services.Interfaces;
using ServerYourWorldMMORPG.Utils;

namespace ServerYourWorldMMORPG
{
	class Program
	{
		public static void Main(string[] args)
		{
			//DatabaseConfirm();
			var serviceProvider = DependencyInjection.BuildServiceProvider();
			//var networkServer = serviceProvider.GetRequiredService<INetworkServer>();

			var commandService = serviceProvider.GetRequiredService<ICommandService>();
			commandService.Initialize();
		}

		private static void DatabaseConfirm()
		{
			Console.Title = "YourWorld Game Server";
			ConsoleUtility.Print("Migrating database...");
			using var dbContext = new ApplicationDbContext();
			dbContext.Database.Migrate();
			ConsoleUtility.Print("Migrating database finished.");
		}
	}
}
