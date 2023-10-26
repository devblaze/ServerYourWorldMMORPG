using ServerYourWorldMMORPG.Utils;
using System.Net.Sockets;
using System.Text;

public class DummyGameClient : IDummyGameClient
{
    private TcpClient _tcpClient;
    private UdpClient _udpClient;
    private NetworkStream stream;

    public DummyGameClient(string serverAddress, int serverPort)
    {
        _tcpClient = new TcpClient(serverAddress, serverPort);
        _udpClient = new UdpClient(serverPort);
        Connect();
    }

    public async Task Connect(string serverAddress, int serverPort)
    {
        _tcpClient = new DummyGameClient(serverAddress, serverPort);
        stream = _tcpClient.GetStream();
        ConsoleUtility.Print("Connected to the server.");
    }

    public async Task ReceivePackets()
    {
        try
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while (true)
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                ConsoleUtility.Print("Received data from server: " + receivedData);
            }
        }
        catch (Exception ex)
        {
            ConsoleUtility.Print("Error receiving data: " + ex.Message);
        }
    }

    public void SendPacket(string data)
    {
        try
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            stream.Write(buffer, 0, buffer.Length);
            ConsoleUtility.Print("Sent data to the server: " + data);
        }
        catch (Exception ex)
        {
            ConsoleUtility.Print("Error sending data: " + ex.Message);
        }
    }

    public void Disconnect()
    {
        if (_tcpClient != null)
        {
            _tcpClient.Close();
            ConsoleUtility.Print("Disconnected from the server.");
        }
    }

    //public static void Main(string[] args)
    //{
    //    DummyGameClient dummyClient = new DummyGameClient();
    //    dummyClient.Connect("127.0.0.1", 8888); // Use your server's IP address and port

    //    // Simulate receiving and sending packets
    //    dummyClient.ReceivePackets();
    //    dummyClient.SendPacket("Hello from dummy client!");

    //    // Keep the client running to receive more packets or add more logic for testing.
    //}
}
