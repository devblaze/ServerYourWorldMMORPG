using ServerYourWorldMMORPG.Models.Utils;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class TCPServer
{
    private TcpListener listener;
    private TcpClient client;
    private UdpClient udpClient;

    public void Start()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        int port = 8888;

        listener = new TcpListener(ipAddress, port);
        listener.Start();
        ConsoleUtility.Print("Server started.");
        ConsoleUtility.Print("Server started. Waiting for connections...");

        // Register the CancelKeyPress event handler
        Console.CancelKeyPress += OnCancelKeyPress;

        // Accept a client connection
        client = listener.AcceptTcpClient();
        ConsoleUtility.Print("Client connected.");

        // Start receiving data from the client
        ReceiveData();
    }

    public void Stop()
    {
        client?.Close();
        listener?.Stop();
        ConsoleUtility.Print("Server has stopped!");
    }

    private void ReceiveData()
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
                ConsoleUtility.Print("Received data: " + data);

                // Here you can implement your server logic to process the received data
            }
        }
        catch (Exception ex)
        {
            ConsoleUtility.Print("Error: " + ex.Message);
        }
    }

    private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        ConsoleUtility.Print("Server shutting down...");
        Stop();
        ConsoleUtility.Print("Server shutdown complete.");
    }
}
