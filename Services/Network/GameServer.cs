using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerYourWorldMMORPG.Services.Network
{
	public class GameServer
	{
		private TcpListener tcpListener;
		private UdpClient udpClient;
		private List<TcpClient> tcpClients = new List<TcpClient>();
		private const int tcpPort = 12345;
		private const int udpPort = 12346;

		public GameServer()
		{
			tcpListener = new TcpListener(IPAddress.Any, tcpPort);
			udpClient = new UdpClient(udpPort);
		}

		public void Start()
		{
			tcpListener.Start();
			AcceptTcpClients();
			StartUdpListener();
		}

		private void AcceptTcpClients()
		{
			Task.Run(async () =>
			{
				while (true)
				{
					TcpClient client = await tcpListener.AcceptTcpClientAsync();
					tcpClients.Add(client);
					HandleTcpClient(client);
				}
			});
		}

		private void HandleTcpClient(TcpClient client)
		{
			// Handle each TCP client in a separate task
			Task.Run(async () =>
			{
				var stream = client.GetStream();
				// Read and process data from the client
			});
		}

		private void StartUdpListener()
		{
			Task.Run(async () =>
			{
				IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
				while (true)
				{
					UdpReceiveResult result = await udpClient.ReceiveAsync();
					// Process UDP data
				}
			});
		}

		public void SendMessage(TcpClient client, string message)
		{
			// Send a message reliably via TCP
			var stream = client.GetStream();
			byte[] buffer = Encoding.UTF8.GetBytes(message);
			stream.Write(buffer, 0, buffer.Length);
		}

		public void Broadcast(string message, bool reliable)
		{
			if (reliable)
			{
				// TCP broadcast to all connected clients
				foreach (var client in tcpClients)
				{
					SendMessage(client, message);
				}
			}
			else
			{
				// UDP broadcast
				byte[] buffer = Encoding.UTF8.GetBytes(message);
				udpClient.Send(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, udpPort));
			}
		}
	}
}
