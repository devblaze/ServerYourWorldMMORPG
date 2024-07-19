//using Microsoft.AspNet.SignalR.Messaging;
//using ServerYourWorldMMORPG.Models.Game.User;
//using ServerYourWorldMMORPG.Services.Network.Transport;
//using ServerYourWorldMMORPG.Services.Network.Transport.TCP;
//using ServerYourWorldMMORPG.Services.Network.Transport.UDP;
//using System.Net;
//using System.Net.Sockets;

//namespace ServerYourWorldMMORPG.Services.Network
//{
//	public class NewGameServerService
//	{
//		private TcpServer _tcpTransport;
//		private UdpServer _udpTransport;
//		private Dictionary<string, UserSession> _connectedUserSessions;
//		private CancellationTokenSource _cancellationTokenSource;
//		private bool _isRunning;

//		public NewGameServerService()
//		{
//			_tcpTransport = new TcpServer();
//			_udpTransport = new UdpServer();
//			_connectedUserSessions = new Dictionary<string, UserSession>();
//			_cancellationTokenSource = new CancellationTokenSource();
//			_isRunning = false;
//		}

//		public async Task StartServer(IPEndPoint endPoint)
//		{
//			_isRunning = true;
//			_tcpTransport.Start(endPoint);
//			_udpTransport.Start(endPoint);

//			while (!_cancellationTokenSource.IsCancellationRequested)
//			{
//				Task.Run(() => Listen());
//			}
//		}

//		public async Task Listen()
//		{
//			if (!_isRunning) return;

//			// Handle TCP data
//			var tcpData = _tcpTransport.ReadAvailableData();
//			foreach (var data in tcpData)
//			{
//				ProcessTcpData(data);
//			}

//			// Handle UDP data
//			var udpData = _udpTransport.ReadAvailableData();
//			foreach (var data in udpData)
//			{
//				ProcessUdpData(data);
//			}
//		}

//		private void ProcessTcpData(TcpData data)
//		{
//			// Example: Deserialize the data into a message object
//			// and handle it based on its type, content, etc.
//			var message = DeserializeMessage(data.Content);
//			HandleMessage(message, data.Client);
//		}

//		private void ProcessUdpData(UdpData data)
//		{
//			// Example: Deserialize the data into a message object
//			// and handle it based on its type, content, etc.
//			var message = DeserializeMessage(data.Content);
//			HandleMessage(message, data.EndPoint);
//		}

//		private Message DeserializeMessage(byte[] data)
//		{
//			// Implementation for deserializing byte data into a Message object
//			// This could involve converting JSON strings to objects, or similar
//		}

//		private void HandleMessage(Message message, TcpClient client)
//		{
//			// Implementation for handling a message from a TCP client
//		}

//		private void HandleMessage(Message message, IPEndPoint endPoint)
//		{
//			// Implementation for handling a message from a UDP endpoint
//		}

//		public async Task SendMessage(MessageType messageType, Message message)
//		{
//			if (_connectedUserSessions.TryGetValue(message.Key, out UserSession userSession))
//			{
//				switch (messageType)
//				{
//					case MessageType.Reliable:
//						_tcpTransport.Send(userSession.TcpClient, message.Value);
//						break;
//					case MessageType.Unreliable:
//						_udpTransport.Send(message.Value, userSession.ClientIpEndPoint);
//						break;
//				}
//			}
//		}

//		public void StopServer()
//		{
//			_isRunning = false;
//			_tcpTransport.Stop();
//			_udpTransport.Stop();
//		}
//	}
//}
