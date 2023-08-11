//using System;
//using System.Net.Sockets;
//using System.Text;

//namespace ServerYourWorldMMORPG.Mocks
//{
//    class UnityClientEmulator
//    {
//        private TcpClient client;
//        private NetworkStream stream;

//        void Main(string[] args)
//        {
//            Console.WriteLine("Testing client!");
//            // TODO: Implement a client console that emulates a Unity Player.
//        }
//    }
//}

//public class TCPClient : MonoBehaviour
//{
//    private TcpClient client;
//    private NetworkStream stream;

//    private void Start()
//    {
//        string serverIP = "127.0.0.1"; // Replace with your server IP address
//        int port = 8888; // Replace with the same port number as the server

//        try
//        {
//            client = new TcpClient(serverIP, port);
//            stream = client.GetStream();
//            Debug.Log("Connected to server.");
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError("Error connecting to server: " + ex.Message);
//        }
//    }

//    private void Update()
//    {
//        // Send data to the server
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            SendData("MOVE 10 20"); // Replace with your desired data format
//        }
//    }

//    private void SendData(string data)
//    {
//        try
//        {
//            byte[] buffer = Encoding.ASCII.GetBytes(data);
//            stream.Write(buffer, 0, buffer.Length);
//            Debug.Log("Sent data: " + data);
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError("Error sending data: " + ex.Message);
//        }
//    }

//    private void OnApplicationQuit()
//    {
//        if (client != null)
//        {
//            client.Close();
//        }
//    }
//}
