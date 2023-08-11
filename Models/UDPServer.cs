using ServerYourWorldMMORPG.Utils;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerYourWorldMMORPG.Models
{
    public class UDPServer
    {
        private UdpClient _udpClient;
        private Thread? _listenThread;
        private CancellationTokenSource? _cancellationTokenSource;
        public int Port { get; private set; }
        public bool isServerRunning { get; private set; }

        public UDPServer(int port)
        {
            isServerRunning = false;
            Port = port;
            _udpClient = new UdpClient(Port);
        }

        public void StartInBackground()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _listenThread = new Thread(() => StartListening(_cancellationTokenSource.Token));
            _listenThread.Start();
        }

        public void StartListening(CancellationToken cancellationToken)
        {
            // Set up UDP listener
            //udpClient = new UdpClient(Port);
            ConsoleUtility.Print("UDP server started at port: " + Port);

            // Start receiving UDP data
            ReceiveUdpData(cancellationToken);
        }

        public void Stop()
        {
            _udpClient?.Close();
            ConsoleUtility.Print("UDP Server has stopped!");
        }

        public void SendUdpData(string message, IPEndPoint remoteEndPoint)
        {
            try
            {
                byte[] sendData = Encoding.ASCII.GetBytes(message);
                _udpClient.Send(sendData, sendData.Length, remoteEndPoint);
                ConsoleUtility.Print("Sent UDP data to client: " + message);
            }
            catch (Exception ex)
            {
                ConsoleUtility.Print("UDP Error: " + ex.Message);
            }
        }

        private void ReceiveUdpData(CancellationToken cancellationToken)
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                while (!cancellationToken.IsCancellationRequested)
                {
                    isServerRunning = true;
                    byte[] receivedBytes = _udpClient.Receive(ref remoteEndPoint);
                    string data = Encoding.ASCII.GetString(receivedBytes);
                    ConsoleUtility.Print("Received UDP data: " + data);

                    // Implement your UDP server logic here...
                }
            }
            catch (Exception ex)
            {
                ConsoleUtility.Print("UDP Error: " + ex.Message);
            }
        }

        //private void ReceiveUdpDataAsync()
        //{
        //    try
        //    {
        //        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        //        while (true)
        //        {
        //            byte[] receivedBytes = await udpClient.ReceiveAsync();
        //            string data = Encoding.ASCII.GetString(receivedBytes);
        //            ConsoleUtility.Print("Received UDP data: " + data);

        //            // Implement your UDP server logic here...
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ConsoleUtility.Print("UDP Error: " + ex.Message);
        //    }
        //}
    }
}
