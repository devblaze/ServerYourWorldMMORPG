using ServerYourWorldMMORPG.Models.Constants;
using ServerYourWorldMMORPG.Utils;
using System.Net.Sockets;
using System.Text;

public class DummyGameClient : IDummyGameClient
{
    private TcpClient _tcpClient;
    private UdpClient _udpClient;
    private NetworkStream stream;
    private bool IsFakeClientOnline = false;

    public DummyGameClient()
    {
        ApplicationSettings.LoadSettings();
    }

    public async void ExecuteCommand(string[] arguments)
    {
        switch (arguments[0])
        {
            case CommandsWordings.START:
                Connect();
                break;
            case CommandsWordings.STOP:
                Disconnect();
                break;
            case CommandsWordings.SEND:
                SendPacket(arguments[1]);
                break;
            default:
                ConsoleUtility.ClientPrint("Help: fakeclient <start|stop|send> <message>");
                break;
        }
    }

    public Task Connect()
    {
        _tcpClient = new TcpClient(ApplicationSettings.IpAddress, ApplicationSettings.TcpPort);
        //_udpClient = new UdpClient(ServerSettings.UdpPort);
        stream = _tcpClient.GetStream(); // Initialize the 'stream' object
        ConsoleUtility.ClientPrint("Connected to the server.");
        IsFakeClientOnline = true;
        return Task.Run(() => ReceivePackets());
    }


    public void ReceivePackets()
    {
        try
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while (IsFakeClientOnline)
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                ConsoleUtility.ClientPrint("Received data from server: " + receivedData);
            }
        }
        catch (Exception ex)
        {
            ConsoleUtility.ClientPrint("Error receiving data: " + ex.Message);
        }
    }

    public void SendPacket(string data)
    {
        if (data == null)
        {
            ConsoleUtility.ClientPrint("Help: fakeclient <start|stop|send> <message>");
            return;
        }

        try
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            stream.Write(buffer, 0, buffer.Length);
            ConsoleUtility.ClientPrint("Sent data to the server: " + data);
        }
        catch (Exception ex)
        {
            ConsoleUtility.ClientPrint("Error sending data: " + ex.Message);
        }
    }

    public void Disconnect()
    {
        IsFakeClientOnline = false;
        if (_tcpClient != null)
        {
            _tcpClient.Close();
            ConsoleUtility.ClientPrint("Disconnected from the server.");
        }
    }
}
