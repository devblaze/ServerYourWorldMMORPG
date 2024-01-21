using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ServerYourWorldMMORPG.Models.Application.Network;
using ServerYourWorldMMORPG.Models.Game;
using ServerYourWorldMMORPG.Models.Game.User;
using ServerYourWorldMMORPG.Services.Application.Interfaces;
using ServerYourWorldMMORPG.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text;

public enum ServerToClient : ushort
{
	playerSpawned = 1,
}

public enum ClientToServerId : ushort
{
	name = 1,
}

namespace ServerYourWorldMMORPG.Services.Application.GameServer
{
	public class GameServerService : IGameServerService
	{
		private TcpListener _tcpListener;
		private UdpClient _udpListener;
		private string name;
		private Dictionary<string, UserSession> _connectedUserSessions;
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

			_tcpListener = new TcpListener(IPAddress.Parse(_gameServerSettings.IpAddress), _loginServerSettings.Port);
			IPEndPoint udpEndPoit = new IPEndPoint(IPAddress.Parse(_gameServerSettings.IpAddress), _gameServerSettings.Port);
			_udpListener = new UdpClient(udpEndPoit);
			_connectedUserSessions = new Dictionary<string, UserSession>();

			//RiptideLogger.Initialize(Console.WriteLine, false);
			//server = new Server();
		}

		public bool ServerStatus()
		{
			return isRunning;
		}

		public async Task StartServer()
		{
			isRunning = true;
			//server.Start((ushort)_gameServerSettings.Port, (ushort)_gameServerSettings.MaxPlayers);
			//server.ClientDisconnected += PlayerLeft;
			//server.ClientConnected += PlayerConnected;

			_tcpListener.Start();
			StartListeningForUdp();
			ConsoleUtility.Print($"{name} started.");

			////Register with the central login server
			////await RegisterWithLoginServer();
			////_tcpListener.BeginAcceptTcpClient();

			while (isRunning)
			{
				CheckClientConnections();
				var client = await _tcpListener.AcceptTcpClientAsync();
				//Task.WhenAll([CheckClientConnections(), OnClientConnect(client)]);
				Task.Run(() => OnClientConnect(client), _cancellationTokenSource.Token);
			}
		}
		public async Task StopServer()
		{
			isRunning = false;
			//server.Stop();
			_udpListener.Close();
			_udpListener?.Dispose();
			_tcpListener.Stop();

			foreach (var userSession in _connectedUserSessions.Values)
			{
				userSession.TcpClient.Close();
			}

			_connectedUserSessions.Clear();
			ConsoleUtility.Print($"{name} stopped.");
		}

		public async Task GetConnectedClients()
		{
			if (_connectedUserSessions.Count != 0)
			{
				foreach (var client in _connectedUserSessions)
				{
					ConsoleUtility.Print($"Connected client: {client.Key} UDP Endpoint: {client.Value.ClientIpEndPoint}");
					if (client.Value.PlayerNetworkObject.NetworkObjectId != null)
					{
						ConsoleUtility.Print($"Network Object ID: {client.Value.PlayerNetworkObject.NetworkObjectId}");
					}
				}
			}
			else
			{
				ConsoleUtility.Print("There are no connected clients yet!");
			}

			//var clients = new List<UserClient>();
			//foreach (var kvp in _connectedUserSessions)
			//{
			//	var client = kvp.Value.TcpClient;
			//	var userClient = new UserClient
			//	{
			//		Id = kvp.Key,
			//		IP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(),
			//		Port = ((IPEndPoint)client.Client.RemoteEndPoint).Port
			//	};
			//	clients.Add(userClient);
			//}
			//return clients;
		}

		public int NumberOfConnectedClients()
		{
			return _connectedUserSessions.Count();
		}

		// TCP Methods START
		private TcpClient? GetTcpClientFromId(string clientId)
		{
			if (_connectedUserSessions.TryGetValue(clientId, out UserSession? userSession))
			{
				if (userSession == null) return null;

				return userSession.TcpClient;
			}

			return null;
		}

		private async Task OnClientConnect(TcpClient client)
		{
			string sessionId = GenerateClientId();
			await ProvideSessionIdToClient(sessionId, true, client);

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
						//ProcessServerMessage(sessionId, message);
					}
				}
			}
			catch (Exception ex)
			{
				ConsoleUtility.Print($"Error with client {sessionId}: {ex}");
			}
			finally
			{
				CleanUpClientResources(sessionId, stream);
			}
		}

		private void CleanUpClientResources(string sessionId, NetworkStream stream)
		{
			if (_connectedUserSessions.TryGetValue(sessionId, value: out UserSession? userSessionToRemove))
			{
				_networkObjectService.RemoveNetworkObject(userSessionToRemove.PlayerNetworkObject);
			}

			if (userSessionToRemove == null) return;

			stream.Close();
			userSessionToRemove.TcpClient.Close();
			_connectedUserSessions.Remove(sessionId);
			ConsoleUtility.Print($"Client {sessionId} has disconnected!");
		}
		// TCP Methods END

		public async Task SendMessage(string sessionId, string message, ProtocolType protocol = ProtocolType.Udp)
		{
			if (protocol == ProtocolType.Tcp)
			{
				var client = GetTcpClientFromId(sessionId);
				if (client == null || !client.Connected) return;

				NetworkStream stream = client.GetStream();
				if (stream.CanWrite)
				{
					byte[] buffer = Encoding.UTF8.GetBytes(message);
					await stream.WriteAsync(buffer, 0, buffer.Length);
					ConsoleUtility.Print($"{protocol}/TCP message: {message}");
				}
			}
			else if (protocol == ProtocolType.Udp)
			{
				if (_connectedUserSessions.TryGetValue(sessionId, out UserSession userSession))
				{
					byte[] data = Encoding.UTF8.GetBytes(message);
					_udpListener.SendAsync(data, data.Length, userSession.ClientIpEndPoint);
					if (!message.Contains("UpdatePosition"))
					{
						ConsoleUtility.Print($"{protocol}/UDP message: {message}");
					}
				}
				else
				{
					ConsoleUtility.Print($"UDP endpoint not found for client: {sessionId}");
				}
			}
		}

		public async Task BroadcastMessage(string message, string exceptUser = null)
		{
			foreach (var client in _connectedUserSessions)
			{
				if (exceptUser == client.Key) continue;
				await SendMessage(client.Key, message);
			}
		}

		private void StartListeningForUdp()
		{
			_udpListener.BeginReceive(OnUdpDataReceived, null);
		}

		private async void OnUdpDataReceived(IAsyncResult result)
		{
			//IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

			//if (_udpListener != null)
			//{
			//	byte[] data = _udpListener.EndReceive(result, ref clientEndPoint);
			//	string message = Encoding.UTF8.GetString(data);
			//	ProcessServerMessage(clientEndPoint, message);
			//	_udpListener.BeginReceive(OnUdpDataReceived, null);
			//}
			//else
			//{
			//	DisconnectClientByIPEndPoint(clientEndPoint);
			//}

			IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
			try
			{
				byte[] data = _udpListener.EndReceive(result, ref clientEndPoint);
				string message = Encoding.UTF8.GetString(data);
				ProcessServerMessage(clientEndPoint, message);
			}
			catch (SocketException ex)
			{
				ConsoleUtility.Print($"SocketException in OnUdpDataReceived: {ex.Message}");
				DisconnectClientByIPEndPoint(clientEndPoint);
			}
			catch (Exception ex)
			{
				ConsoleUtility.Print($"Exception in OnUdpDataReceived: {ex.Message}");
				DisconnectClientByIPEndPoint(clientEndPoint);
			}
			finally
			{
				if (_udpListener != null)
				{
					_udpListener.BeginReceive(OnUdpDataReceived, null);
				}
			}
		}

		private async Task ProcessServerMessage(IPEndPoint clientEndPoint, string message)
		{
			if (!message.Contains("UpdatePosition"))
			{
				ConsoleUtility.Print($"Received UDP message from {clientEndPoint}: {message}");
			}

			if (message == "RequestSessionId" && !IsEndPointDuplicate(clientEndPoint))
			{
				await ClientRequsestedSessionId(clientEndPoint);
			}

			string[] splitMessage = message.Split('|');
			string sessionId = splitMessage[0];

			if (IsSessionIdValid(sessionId))
			{
				if (_connectedUserSessions.TryGetValue(sessionId, value: out UserSession userSession))
				{
					if (userSession.ClientIpEndPoint != clientEndPoint)
					{
						userSession.LastMessageReceived = DateTime.Now;

						ProcessServerMessageForLoggedInUser(sessionId, splitMessage);
					}
					else
					{
						ConsoleUtility.Print($"!!!!!! Hacker detected with IP: {clientEndPoint.Address} Port: {clientEndPoint.Port}");
					}
				}
			}
		}

		private bool IsEndPointDuplicate(IPEndPoint clientEndPoint)
		{
			bool endPointExistance = _connectedUserSessions.Values
				.Where(session => session.ClientIpEndPoint == clientEndPoint)
				.Count() > 1;

			return endPointExistance;
		}

		private async Task ProcessServerMessageForLoggedInUser(string sessionId, string[] splitMessage)
		{
			string command = splitMessage[1];

			switch (command)
			{
				case "UpdatePosition":
					UpdatePosition(splitMessage[2]);
					break;
				case "PlayerConnected":
					PlayerSpawnRequest(sessionId);
					break;
				case "PlayerDisconnected":
					PlayerDisconnect(sessionId);
					break;
				case "HeartBeat":
					HeartBeatPacket(sessionId);
					break;
				case "PlayersConnected":
					string message = $"PlayersConnected|{NumberOfConnectedClients()}";
					SendMessage(sessionId, message);
					break;
				default:
					ConsoleUtility.Print($"Unknown packet received: {splitMessage} From: {sessionId}");
					break;
			}
		}

		public void CheckClientConnections()
		{
			foreach (var session in _connectedUserSessions)
			{
				if (DateTime.UtcNow - session.Value.LastMessageReceived > TimeSpan.FromSeconds(30))
				{
					ConsoleUtility.Print($"Missed heartbeat disconnecting client: {session.Key}");
					PlayerDisconnect(session.Key);
				}
			}
		}

		private bool IsSessionIdValid(string sessionId)
		{
			if (_connectedUserSessions.TryGetValue(sessionId, value: out UserSession? userSession))
			{
				return true;
			}

			return false;
		}

		private async Task HeartBeatPacket(string sessionId)
		{
			CheckClientConnections();
			await SendMessage(sessionId, "HeartBeatReceived");
		}

		private async Task UpdatePosition(string message)
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

		private async void PlayerDisconnect(string sessionId)
		{
			if (_connectedUserSessions.TryGetValue(sessionId, value: out UserSession userSession))
			{
				if (userSession.PlayerNetworkObject != null)
				{
					string message = $"ClientDisconnect|{userSession.PlayerNetworkObject.NetworkObjectId}";
					await BroadcastMessage(message, sessionId);
					_networkObjectService.RemoveNetworkObject(userSession.PlayerNetworkObject);
					_connectedUserSessions.Remove(sessionId);
				}
			}
		}

		public async Task DisconnectClientByIPEndPoint(IPEndPoint clientEndPoint)
		{
			var sessionToDisconnect = _connectedUserSessions.Values
				.FirstOrDefault(session => session.ClientIpEndPoint.Equals(clientEndPoint));

			if (sessionToDisconnect != null)
			{
				sessionToDisconnect.TcpClient?.Close();

				string sessionId = GetSessionIdByEndPoint(clientEndPoint);
				if (!string.IsNullOrEmpty(sessionId))
				{
					string message = $"ClientDisconnect|{sessionToDisconnect.PlayerNetworkObject.NetworkObjectId}";
					await BroadcastMessage(message, sessionId);
					_connectedUserSessions.Remove(sessionId);
				}
			}
		}

		public string GetSessionIdByEndPoint(IPEndPoint clientEndPoint)
		{
			foreach (var session in _connectedUserSessions)
			{
				if (session.Value.ClientIpEndPoint.Equals(clientEndPoint))
				{
					return session.Key;  // Return the session ID (key)
				}
			}
			return null;  // Return null if no matching session is found
		}

		private string GenerateClientId()
		{
			string clientId = Guid.NewGuid().ToString();
			if (_connectedUserSessions.TryGetValue(clientId, out UserSession? existsingUserSession))
			{
				clientId = Guid.NewGuid().ToString();
			}

			return clientId;
		}

		private async Task ProvideSessionIdToClient(string sessionId, bool isTcp = false, TcpClient? client = null, IPEndPoint? clientEndPoint = null)
		{
			if (client == null && clientEndPoint == null)
			{
				ConsoleUtility.Print("You need to put TcpClient or clientEndPoit to provide Session ID!");
				return;
			}

			UserSession userSession;
			ProtocolType protocol;
			if (isTcp && client != null)
			{
				userSession = new UserSession { TcpClient = client };
				protocol = ProtocolType.Tcp;
			}
			else
			{
				userSession = new UserSession { ClientIpEndPoint = clientEndPoint };
				protocol = ProtocolType.Udp;
			}

			_connectedUserSessions.Add(sessionId, userSession);
			await SendMessage(sessionId, $"SessionId|{sessionId}", protocol);
			ConsoleUtility.Print($"Client connected: {sessionId} and has been informed of the session ID");
		}

		private async Task ClientRequsestedSessionId(IPEndPoint clientEndPoint)
		{
			string sessionId = GenerateClientId();
			await ProvideSessionIdToClient(sessionId, false, null, clientEndPoint);
		}

		// Currently not in use
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
