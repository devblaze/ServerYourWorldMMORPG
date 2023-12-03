using Microsoft.Extensions.DependencyInjection;
using ServerYourWorldMMORPG.Services.Application.Interfaces;

namespace ServerYourWorldMMORPG
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.Title = "YourWorldMMORPG Server";
			var serviceProvider = DependencyInjection.BuildServiceProvider();
			var commandService = serviceProvider.GetRequiredService<ICommandService>();
			commandService.Initialize();
		}
	}
}
