using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ServerYourWorldMMORPG.Models.Application.Network;
using ServerYourWorldMMORPG.Models.Game;
using ServerYourWorldMMORPG.Models.Game.User;
using ServerYourWorldMMORPG.Services.Application.Interfaces;
using ServerYourWorldMMORPG.Utils;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;

namespace ServerYourWorldMMORPG.Services.Application.GameServer
{
	public class GameServerService : IGameServerService
	{
		private TcpListener _tcpListener;
		private UdpClient _udpListener;
		private string name;
		private Dictionary<string, UserSession> _connectedUserSessions = new Dictionary<string, UserSession>();
		private bool isRunning;
		private GameServerSettings _gameServerSettings;
		private LoginServerSettings _loginServerSettings;
		private INetworkObjectService _networkObjectService;
		private CancellationTokenSource _cancellationTokenSource;

		public GameServerService(IOptions<GameServerSettings> gameServerSettings,
			IOptions<LoginServerSettings> loginServerSettings,
			INetworkObjectService networkObjectService)
		{
			_cancellationTokenSource = new CancellationTokenSource();
			_gameServerSettings = gameServerSettings.Value;
			_loginServerSettings = loginServerSettings.Value;
			_networkObjectService = networkObjectService;

			name = _gameServerSettings.Name;
			isRunning = false;

			_tcpListener = new TcpListener(IPAddress.Parse(_gameServerSettings.IpAddress), _gameServerSettings.Port);
			IPEndPoint udpEndPoit = new IPEndPoint(IPAddress.Parse(_gameServerSettings.IpAddress), _gameServerSettings.Port);
			_udpListener = new UdpClient(udpEndPoit);
			StartListeningForUdp();
		}

		public bool ServerStatus()
		{
			return isRunning;
		}

		public async Task StartServer()
		{
			isRunning = true;
			_tcpListener.Start();
			_udpListener.BeginReceive(OnUdpDataReceived, null);
			ConsoleUtility.Print($"{name} started.");

			// Register with the central login server
			//await RegisterWithLoginServer();

			while (isRunning)
			{
				var client = await _tcpListener.AcceptTcpClientAsync();
				Task.Run(() => OnClientConnect(client), _cancellationTokenSource.Token);
			}
		}

		public async Task StopServer()
		{
			isRunning = false;
			_tcpListener.Stop();

			foreach (var userSession in _connectedUserSessions.Values)
			{
				userSession.TcpClient.Close();
			}

			_connectedUserSessions.Clear();
			ConsoleUtility.Print($"{name} stopped.");
		}

		public List<UserClient> GetConnectedClients()
		{
			var clients = new List<UserClient>();
			foreach (var kvp in _connectedUserSessions)
			{
				var client = kvp.Value.TcpClient;
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

		public async Task SendMessage(string clientId, string message, ProtocolType protocol = ProtocolType.Udp)
		{
			if (protocol == ProtocolType.Tcp)
			{
				var client = GetTcpClientFromId(clientId);
				if (client == null || !client.Connected) return;

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
					ConsoleUtility.Print($"Error sending TCP message to client: {ex.Message}");
				}
			}
			else if (protocol == ProtocolType.Udp)
			{
				if (_connectedUserSessions.TryGetValue(clientId, out var userSession))
				{
					byte[] data = Encoding.UTF8.GetBytes(message);
					_udpListener.Send(data, data.Length, userSession.UdpEndPoint);
				}
				else
				{
					ConsoleUtility.Print($"UDP endpoint not found for client: {clientId}");
				}
			}
		}

		public async Task BroadcastMessage(string message)
		{
			foreach (var client in _connectedUserSessions)
			{
				await SendMessage(client.Key, message);
			}
		}

		private void StartListeningForUdp()
		{
			_udpListener.BeginReceive(OnUdpDataReceived, null);
		}

		private void OnUdpDataReceived(IAsyncResult result)
		{
			try
			{
				IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = _udpListener.EndReceive(result, ref clientEndPoint);

				// Process the received data
				string message = Encoding.UTF8.GetString(data);
				ConsoleUtility.Print($"Received UDP message from {clientEndPoint}: {message}");

				var clientEntry = _connectedUserSessions.FirstOrDefault(entry => entry.Value.UdpEndPoint.Equals(clientEndPoint));
				if (clientEntry.Value != null)
				{
					string cliendId = clientEntry.Key;
					ProcessServerMessage(cliendId, message);
				}

				_udpListener.BeginReceive(OnUdpDataReceived, null);
			}
			catch (Exception ex)
			{
				ConsoleUtility.Print($"UDP receive error: {ex.Message}");
			}
		}

		private TcpClient? GetTcpClientFromId(string clientId)
		{
			if (_connectedUserSessions.TryGetValue(clientId, out UserSession userSession))
			{
				return userSession.TcpClient;
			}

			return null;
		}

		private async Task OnClientConnect(TcpClient client)
		{
			string clientId = GenerateClientId();
			UserSession userSession = new UserSession { TcpClient = client };
			_connectedUserSessions.Add(clientId, userSession);
			await SendMessage(clientId, $"SessionId|{clientId}");
			ConsoleUtility.Print($"Client connected: {clientId} and has been informed of the session ID");

			NetworkStream stream = client.GetStream();

			byte[] buffer = new byte[4096];
			int bytesRead;
			try
			{
				while (!_cancellationTokenSource.IsCancellationRequested)
				{
					if ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
					{
						string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
						//ConsoleUtility.Print($"Message: {message} From: {clientId}");
						ProcessServerMessage(clientId, message);
					}
				}
			}
			catch (Exception ex)
			{
				ConsoleUtility.Print($"Error with client {clientId}: {ex}");
			}
			finally
			{
				CleanUpClientResources(clientId, stream);
			}
		}

		private void CleanUpClientResources(string clientId, NetworkStream stream)
		{
			if (_connectedUserSessions.TryGetValue(clientId, value: out var userSessionToRemove))
			{
				_networkObjectService.RemoveNetworkObject(userSessionToRemove.PlayerNetworkObject);
			}
			stream.Close();
			userSessionToRemove.TcpClient.Close();
			_connectedUserSessions.Remove(clientId);
			ConsoleUtility.Print($"Client {clientId} has disconnected!");
		}

		private void ProcessServerMessage(string clientId, string message)
		{
			string[] splitMessage = message.Split('|');
			string command = splitMessage[0];
			string data = splitMessage[1];

			switch (command)
			{
				case "UpdatePosition":
					UpdatePosition(clientId, data);
					break;
				case "NewPlayer":
					string newPlayerId = splitMessage[1];
					Vector3 spawnPos = new Vector3(float.Parse(splitMessage[2]), float.Parse(splitMessage[3]), float.Parse(splitMessage[4]));
					//NotifyClientToSpawnPlayer(newPlayerId);
					break;
				//case command.StartsWith('PlayerConnected'):
				case "PlayerConnected":
					PlayerSpawnRequest(data);
					break;
				default:
					string data2 = splitMessage[1];
					ConsoleUtility.Print($"Unknown packet received: {data2} From: {clientId}");
					break;
			}
		}

		private async Task UpdatePosition(string clientId, string message)
		{
			string updatePositions = _networkObjectService.UpdateNetworkObjectWithMessage(message);
			await BroadcastMessage($"UpdatePosition|{updatePositions}");
		}

		private async void PlayerSpawnRequest(string sessionId)
		{
			if (_connectedUserSessions.TryGetValue(sessionId, value: out var userSession))
			{
				NetworkObject networkObject = _networkObjectService.InitializeNetworkObject();
				userSession.PlayerNetworkObject = networkObject;

				string message = JsonConvert.SerializeObject(networkObject);
				string networkObjectsMessage = _networkObjectService.NetworkObjectsListToJsonMessage();

				await SendMessage(sessionId, $"PlayerSpawn|{message}");
				await SendMessage(sessionId, $"RemotePlayerSpawn|{networkObjectsMessage}");
			}
		}

		private string GenerateClientId()
		{
			string clientId = Guid.NewGuid().ToString();
			if (_connectedUserSessions.TryGetValue(clientId, out UserSession? userSession))
			{
				clientId = Guid.NewGuid().ToString();
			}
			return clientId;
		}

		private async Task RegisterWithLoginServer()
		{
			using var tcpClient = new TcpClient();
			await tcpClient.ConnectAsync(_loginServerSettings.IpAddress, _loginServerSettings.Port);

			using var networkStream = tcpClient.GetStream();
			GameServerSettings info = new GameServerSettings
			{
				Name = name,
				IpAddress = ((IPEndPoint)_tcpListener.LocalEndpoint).Address.ToString(),
				Port = ((IPEndPoint)_tcpListener.LocalEndpoint).Port,
				CurrentPlayerCount = 0, // Example, you need to implement player counting
				MaxPlayers = 100 // Example, should be a configurable value
			};

			string json = JsonConvert.SerializeObject(info);
			byte[] bytes = Encoding.UTF8.GetBytes(json);
			await networkStream.WriteAsync(bytes, 0, bytes.Length);
		}
	}
}
