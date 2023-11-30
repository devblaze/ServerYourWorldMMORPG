using Microsoft.Extensions.Options;
using ServerYourWorldMMORPG.Models.Application.Network;
using ServerYourWorldMMORPG.Services.Application.Interfaces;
using ServerYourWorldMMORPG.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ServerYourWorldMMORPG.Services.Application.GameServer
{
	public class GameServerService : IGameServerService
	{
		private TcpListener tcpListener;
		private string name;
		private Dictionary<string, TcpClient> _connectedClients = new Dictionary<string, TcpClient>();
		private bool isRunning;
		private GameServerSettings _gameServerSettings;
		private LoginServerSettings _loginServerSettings;

		public GameServerService(IOptions<GameServerSettings> gameServerSettings, IOptions<LoginServerSettings> loginServerSettings)
		{
			_gameServerSettings = gameServerSettings.Value;
			_loginServerSettings = loginServerSettings.Value;
			name = _gameServerSettings.Name;
			isRunning = false;
			tcpListener = new TcpListener(IPAddress.Parse(_gameServerSettings.IpAddress), _gameServerSettings.Port);
		}

		public bool ServerStatus()
		{
			return isRunning;
		}

		public async Task StartServer()
		{
			isRunning = true;
			tcpListener.Start();
			ConsoleUtility.Print($"{name} started.");

			// Register with the central login server
			//await RegisterWithLoginServer();

			while (isRunning)
			{
				var client = tcpListener.AcceptTcpClient();
				Task.Run(() => HandleClient(client));
			}
		}

		public async Task StopServer()
		{
			isRunning = false;
			tcpListener.Stop();

			foreach (var client in _connectedClients.Values)
			{
				client.Close();
			}

			_connectedClients.Clear();
			ConsoleUtility.Print($"{name} stopped.");
		}

		public List<UserClient> GetConnectedClients()
		{
			var clients = new List<UserClient>();
			foreach (var kvp in _connectedClients)
			{
				var client = kvp.Value;
				var userClient = new UserClient
				{
					Id = kvp.Key,
					IP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(),
					Port = ((IPEndPoint)client.Client.RemoteEndPoint).Port
				};
				clients.Add(userClient);
			}
			return clients;
		}

		private void NotifyClientToSpawnPlayer(TcpClient client)
		{
			string message = "SpawnPlayer";
			SendMessage(client, message);
			ConsoleUtility.Print("Sent spawn message to client.");
		}

		public async Task SendMessageToClientById(string clientId, string message)
		{
			if (_connectedClients.TryGetValue(clientId, out TcpClient client))
			{
				await SendMessage(client, message);
			}
			else
			{
				ConsoleUtility.Print($"No client found with ID: {clientId}");
			}
		}

		private async Task SendMessage(TcpClient client, string message)
		{
			if (client == null || !client.Connected)
				return;

			try
			{
				NetworkStream stream = client.GetStream();
				if (stream.CanWrite)
				{
					byte[] buffer = Encoding.UTF8.GetBytes(message);
					await stream.WriteAsync(buffer, 0, buffer.Length);
				}
			}
			catch (Exception ex)
			{
				ConsoleUtility.Print($"Error sending message to client: {ex.Message}");
				// Additional error handling as needed
			}
		}

		private async Task HandleClient(TcpClient client)
		{
			// Generate a unique ID for the client
			string clientId = GenerateClientId(client);
			_connectedClients.Add(clientId, client);
			ConsoleUtility.Print($"Client connected: {clientId}");

			NetworkStream stream = client.GetStream();

			// Listen for messages from the client
			byte[] buffer = new byte[1024];
			int bytesRead;

			try
			{
				NotifyClientToSpawnPlayer(client);

				while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
				{
					string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
					BroadcastMessage(clientId, message);
				}
			}
			catch
			{
				// Handle errors and client disconnection
			}
			finally
			{
				// Remove the client from the list and close the connection
				_connectedClients.Remove(clientId);
				client.Close();
			}
		}

		private string GenerateClientId(TcpClient client)
		{
			return client.Client.RemoteEndPoint.ToString(); // Example unique ID
		}

		private void BroadcastMessage(string sourceClientId, string message)
		{
			foreach (var kvp in _connectedClients)
			{
				if (kvp.Key != sourceClientId) // Don't send back to the source client
				{
					TcpClient otherClient = kvp.Value;
					NetworkStream stream = otherClient.GetStream();
					byte[] buffer = Encoding.UTF8.GetBytes(message);
					stream.WriteAsync(buffer, 0, buffer.Length); // Send the message asynchronously
				}
			}
		}

		private async Task RegisterWithLoginServer()
		{
			using var tcpClient = new TcpClient();
			await tcpClient.ConnectAsync(_loginServerSettings.IpAddress, _loginServerSettings.Port);

			using var networkStream = tcpClient.GetStream();
			GameServerSettings info = new GameServerSettings
			{
				Name = name,
				IpAddress = ((IPEndPoint)tcpListener.LocalEndpoint).Address.ToString(),
				Port = ((IPEndPoint)tcpListener.LocalEndpoint).Port,
				CurrentPlayerCount = 0, // Example, you need to implement player counting
				MaxPlayers = 100 // Example, should be a configurable value
			};

			string json = JsonSerializer.Serialize(info);
			byte[] bytes = Encoding.UTF8.GetBytes(json);
			await networkStream.WriteAsync(bytes, 0, bytes.Length);
		}
	}
}
