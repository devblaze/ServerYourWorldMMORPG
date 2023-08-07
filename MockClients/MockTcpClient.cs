using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace ServerYourWorldMMORPG.MockClients
{
    public class MockTcpClient
    {
        private TcpClient client;

        public MockTcpClient()
        {
            client = new TcpClient();
        }

        public void Connect(string ipAddress, int port)
        {
            client.Connect(ipAddress, port);
        }

        public void SendData(string data)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            stream.Write(buffer, 0, buffer.Length);
        }

        public string ReceiveData()
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }

        public void Disconnect()
        {
            client.Close();
        }

        public void RunInNewTerminal()
        {
            // Create a new process to run the mock client
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/k dotnet run --project C:\\Users\\nikos\\Documents\\Visual Studio Projects\\ServerYourWorldMMORPG\\MockClients";
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(typeof(MockTcpClient).Assembly.Location);
            process.Start();

            // Wait for the process to exit (terminal is closed)
            process.WaitForExit();

            // Clean up resources after terminal is closed
            // For example, you can disconnect the client here
            Disconnect();
        }

        public void StartInNewTerminal()
        {
            // Create a new process to run the mock client
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/k dotnet run --project C:\\Users\\nikos\\Documents\\Visual Studio Projects\\ServerYourWorldMMORPG\\MockClients";
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(typeof(MockTcpClient).Assembly.Location);
            process.Start();

            // Do not wait for the process to exit; let it run in the new terminal
        }
    }
}
