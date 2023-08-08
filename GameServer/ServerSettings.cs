using Microsoft.Extensions.Configuration;
using ServerYourWorldMMORPG.Utils;

namespace ServerYourWorldMMORPG.GameServer
{
    public class ServerSettings
    {
        public string IpAddress { get; set; }
        public int TcpPort { get; set; }
        public int UdpPort { get; set; }
        public int MaxPlayers { get; set; }

        public static ServerSettings LoadSettings()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(@"C:\\Users\\nikos\\Documents\\Visual Studio Projects\\ServerYourWorldMMORPG\\appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var serverSettings = configuration.GetSection("ServerSettings");

            //Console.WriteLine($"Current working directory: {Environment.CurrentDirectory}");

            string ipAddress = serverSettings["IpAddress"];
            int tcpPort = int.Parse(serverSettings["TcpPort"]);
            int udpPort = int.Parse(serverSettings["UdpPort"]);
            int maxPlayers = int.Parse(serverSettings["MaxPlayers"]);

            //object[] array = { ipAddress, tcpPort, udpPort, maxPlayers };
            //ConsoleUtility.DebugPrint(array);

            return new ServerSettings
            {
                IpAddress = ipAddress,
                TcpPort = tcpPort,
                UdpPort = udpPort,
                MaxPlayers = maxPlayers
            };
        }
    }
}
