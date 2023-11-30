using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServerYourWorldMMORPG.Database;
using ServerYourWorldMMORPG.Models.Application.Network;
using ServerYourWorldMMORPG.Models.Game.User;
using ServerYourWorldMMORPG.Services.Application.Interfaces;
using ServerYourWorldMMORPG.Utils;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ServerYourWorldMMORPG.Services.Application.LoginServer
{
	public class LoginServerService : ILoginServerService
	{
		private TcpListener tcpListener;
		private readonly ApplicationDbContext _context;
		private readonly LoginServerSettings _settings;
		private List<GameServerSettings> availableGameServers;
		private bool isRunning;

		public LoginServerService(ApplicationDbContext context, IOptions<LoginServerSettings> settings)
		{
			_settings = settings.Value;
			_context = context;
			tcpListener = new TcpListener(IPAddress.Any, _settings.Port);
			availableGameServers = new List<GameServerSettings>();
			isRunning = false;
		}

		public void PrintAvailableGameServers()
		{
			if (availableGameServers.Count == 0)
			{
				ConsoleUtility.Print("There are no registered servers.");
				return;
			}

			foreach (var server in availableGameServers)
			{
				ConsoleUtility.Print($"Server Name: {server.Name}, IP: {server.IpAddress}, Port: {server.Port}, Players: {server.CurrentPlayerCount}/{server.MaxPlayers}");
			}
		}

		public async Task StartServer()
		{
			tcpListener.Start();
			ConsoleUtility.Print("Login server started.");
			isRunning = true;

			while (isRunning)
			{
				var client = await tcpListener.AcceptTcpClientAsync();
				HandleClientAsync(client);
			}
		}

		public async Task StopServer()
		{
			isRunning = false;
			tcpListener.Stop();
			ConsoleUtility.Print("Login server stopped.");
		}

		public bool ServerStatus()
		{
			return isRunning;
		}

		public void RegisterGameServer(GameServerSettings serverInfo)
		{
			availableGameServers.Add(serverInfo);
			// You'd typically notify clients of the new server here or refresh the list
		}

		private async Task HandleClientAsync(TcpClient client)
		{
			using (var networkStream = client.GetStream())
			{
				byte[] buffer = new byte[1024];
				int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
				string credentialsJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);
				var credentials = JsonSerializer.Deserialize<Account>(credentialsJson);

				if (await AuthenticateUser(credentials))
				{
					string gameServersJson = JsonSerializer.Serialize(availableGameServers);
					byte[] gameServersBytes = Encoding.UTF8.GetBytes(gameServersJson);

					await networkStream.WriteAsync(gameServersBytes, 0, gameServersBytes.Length);
				}
				else
				{
					byte[] failMessage = Encoding.UTF8.GetBytes("Authentication failed.");
					await networkStream.WriteAsync(failMessage, 0, failMessage.Length);
				}
			}
			client.Close();
		}

		private async Task<bool> AuthenticateUser(Account credentials)
		{
			var userAccount = await _context.Accounts
										   .FirstOrDefaultAsync(a => a.Username == credentials.Username);

			if (userAccount != null)
			{
				string hashedPassword = PasswordHash(credentials.Password);
				return userAccount.Password == hashedPassword;
			}

			return false;
		}

		private string PasswordHash(string password)
		{
			// Implement the hashing logic that was used to store the passwords
			// For example, using SHA-256, but consider a stronger hash function like bcrypt or Argon2
			using (var sha256 = SHA256.Create())
			{
				byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
				return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
			}
		}
	}
}
