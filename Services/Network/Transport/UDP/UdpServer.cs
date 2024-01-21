using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace ServerYourWorldMMORPG.Services.Network.Transport.UDP
{
	public class UdpServer
	{
		private UdpClient _udpClient;
		private CancellationTokenSource _cancellationTokenSource;
		private ConcurrentQueue<UdpReceiveResult> _receivedData = new ConcurrentQueue<UdpReceiveResult>();

		public UdpServer()
		{
			_cancellationTokenSource = new CancellationTokenSource();
		}

		public void Start(IPEndPoint endPoint)
		{
			_udpClient = new UdpClient(endPoint);
			ReceiveUdpDataAsync(_cancellationTokenSource.Token);
		}

		public async Task Send(byte[] data, IPEndPoint remoteEndPoint)
		{
			if (_udpClient == null)
				return;

			await _udpClient.SendAsync(data, data.Length, remoteEndPoint);
		}

		public IEnumerable<UdpReceiveResult> ReadAvailableData()
		{
			while (_receivedData.TryDequeue(out var data))
			{
				yield return data;
			}
		}

		public void Stop()
		{
			_cancellationTokenSource.Cancel();
			_udpClient.Close();
			_udpClient?.Dispose();
		}

		private async Task ReceiveUdpDataAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				UdpReceiveResult result = await _udpClient.ReceiveAsync();
				_receivedData.Enqueue(result);
			}
		}
	}
}
