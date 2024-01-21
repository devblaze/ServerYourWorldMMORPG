using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace ServerYourWorldMMORPG.Services.Network.Transport.TCP
{
	public class TcpServer
	{
		private TcpListener _tcpListener;
		private CancellationTokenSource _cancellationTokenSource;
		private Dictionary<TcpClient, ConcurrentQueue<byte[]>> _receivedData = new Dictionary<TcpClient, ConcurrentQueue<byte[]>>();

		public TcpServer()
		{
			_cancellationTokenSource = new CancellationTokenSource();
		}

		public void Start(IPEndPoint endPoint)
		{
			_tcpListener = new TcpListener(endPoint);
			_tcpListener.Start();
			AcceptClientsAsync(_cancellationTokenSource.Token);
		}

		public void Stop()
		{
			_cancellationTokenSource.Cancel();
			_tcpListener.Stop();
		}

		public async Task Send(TcpClient client, byte[] data)
		{
			if (client == null || !client.Connected)
				return;

			NetworkStream stream = client.GetStream();
			if (stream.CanWrite)
			{
				await stream.WriteAsync(data, 0, data.Length);
			}
		}

		public Dictionary<TcpClient, IEnumerable<byte[]>> ReadAvailableData()
		{
			var result = new Dictionary<TcpClient, IEnumerable<byte[]>>();
			foreach (var kvp in _receivedData)
			{
				var dataList = new List<byte[]>();
				while (kvp.Value.TryDequeue(out var data))
				{
					dataList.Add(data);
				}

				result[kvp.Key] = dataList;
			}

			return result;
		}

		private async Task AcceptClientsAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				TcpClient client = await _tcpListener.AcceptTcpClientAsync();
				HandleClientAsync(client, cancellationToken);
			}
		}

		private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
		{
			ConcurrentQueue<byte[]> clientQueue = new ConcurrentQueue<byte[]>();
			_receivedData[client] = clientQueue;

			using (client)
			using (NetworkStream stream = client.GetStream())
			{
				byte[] buffer = new byte[1024]; // Adjust buffer size as needed
				while (!cancellationToken.IsCancellationRequested)
				{
					int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
					if (bytesRead > 0)
					{
						byte[] data = new byte[bytesRead];
						Array.Copy(buffer, data, bytesRead);
						clientQueue.Enqueue(data);
					}
				}
			}

			_receivedData.Remove(client);
		}
	}
}
