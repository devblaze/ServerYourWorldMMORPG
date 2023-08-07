using Org.BouncyCastle.Utilities.Net;
using ServerYourWorldMMORPG.Models.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text;
using IPAddress = System.Net.IPAddress;

namespace ServerYourWorldMMORPG.Models.Tcp
{
    public class TCPServer
    {
        private TcpListener listener;
        private Dictionary<string, TcpClient> connectedClients = new Dictionary<string, TcpClient>();
        public string IpString { get; private set; }
        public int Port { get; private set; }
        public int MaxPlayers { get; private set; }

        public TCPServer(string IpString, int port, int maxPlayers)
        {
            this.IpString = IpString;
            this.Port = port;
            this.MaxPlayers = maxPlayers;
        }

        public void Start()
        {
            //StartListening();
            IPAddress ipAddress = IPAddress.Parse(IpString);

            listener = new TcpListener(ipAddress, Port);
            listener.Start();
            ConsoleUtility.Print("Server started. Waiting for connections...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ConsoleUtility.Print("Client connected: " + client.Client.RemoteEndPoint);

                string clientId = Guid.NewGuid().ToString(); // Generate a unique identifier
                connectedClients[clientId] = client;

                ReceiveData(clientId, client);
            }
        }

        public void Stop()
        {
            foreach (var client in connectedClients.Values)
            {
                client.Close();
            }
            listener?.Stop();
            ConsoleUtility.Print("TCP Server has stopped!");
        }

        //private void StartListening(IPAddress ipAddress)
        //{
        //    IPAddress ipAddress = IPAddress.Parse(IpAddress);

        //    listener = new TcpListener(ipAddress, Port);
        //    listener.Start();
        //    ConsoleUtility.Print("Server started. Waiting for connections...");

        //    while (true)
        //    {
        //        TcpClient client = listener.AcceptTcpClient();
        //        ConsoleUtility.Print("Client connected: " + client.Client.RemoteEndPoint);

        //        string clientId = Guid.NewGuid().ToString(); // Generate a unique identifier
        //        connectedClients[clientId] = client;

        //        ReceiveData(clientId, client);
        //    }
        //}

        private void ReceiveData(string clientId, TcpClient client)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    ConsoleUtility.Print($"Received data from client {clientId}: {data}");

                    ProcessData(clientId, data);
                }
            }
            catch (Exception ex)
            {
                ConsoleUtility.Print($"Error for client {clientId}: {ex.Message}");
                connectedClients.Remove(clientId);
            }
        }

        private void ProcessData(string clientId, string data)
        {
            // Process the received data
            // ... (your game logic)

            // Example: Sending a response to the client
            SendTcpData(clientId, "Hello, client " + clientId);
        }

        private void SendTcpData(string clientId, string message)
        {
            if (connectedClients.TryGetValue(clientId, out TcpClient client))
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.ASCII.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    ConsoleUtility.Print($"Sent TCP data to client {clientId}: {message}");
                }
                catch (Exception ex)
                {
                    ConsoleUtility.Print($"TCP Error for client {clientId}: {ex.Message}");
                }
            }
        }
    }
}
