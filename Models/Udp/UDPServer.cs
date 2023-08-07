using ServerYourWorldMMORPG.Models.Utils;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerYourWorldMMORPG.Models.Udp
{
    public class UDPServer
    {
        private UdpClient udpClient;
        public int Port { get; private set; }

        public UDPServer(int port)
        {
            Port = port;
        }

        public void Start()
        {
            // Set up UDP listener
            udpClient = new UdpClient(Port);
            ConsoleUtility.Print("UDP listener started. Waiting for UDP packets...");

            // Start receiving UDP data
            ReceiveUdpData();
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

        private void ReceiveUdpData()
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                while (true)
                {
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
    }
}
