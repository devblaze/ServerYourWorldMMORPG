using Microsoft.Extensions.DependencyInjection;
using ServerYourWorldMMORPG.Services.Application.Interfaces;

namespace ServerYourWorldMMORPG
{
	class Program
	{
		public static void Main(string[] args)
		{
			var serviceProvider = DependencyInjection.BuildServiceProvider();
			var commandService = serviceProvider.GetRequiredService<ICommandService>();
			commandService.Initialize();
		}
	}
}
