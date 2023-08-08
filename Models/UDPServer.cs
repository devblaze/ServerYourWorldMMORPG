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
        private UdpClient udpClient;
        private Thread? listenThread;
        private CancellationTokenSource? cancellationTokenSource;
        public int Port { get; private set; }
        public bool isServerRunning { get; private set; }

        public UDPServer(int port)
        {
            isServerRunning = false;
            Port = port;
            udpClient = new UdpClient(Port);
        }

        public void StartInBackground()
        {
            cancellationTokenSource = new CancellationTokenSource();
            listenThread = new Thread(() => StartListening(cancellationTokenSource.Token));
            listenThread.Start();
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
            udpClient?.Close();
            ConsoleUtility.Print("UDP Server has stopped!");
        }

        public void SendUdpData(string message, IPEndPoint remoteEndPoint)
        {
            try
            {
                byte[] sendData = Encoding.ASCII.GetBytes(message);
                udpClient.Send(sendData, sendData.Length, remoteEndPoint);
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
                    byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
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
