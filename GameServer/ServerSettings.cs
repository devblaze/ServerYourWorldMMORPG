using Microsoft.Extensions.Configuration;

namespace ServerYourWorldMMORPG.GameServer
{
    public static class ServerSettings
    {
        public static string IpAddress { get; set; }
        public static int TcpPort { get; set; }
        public static int UdpPort { get; set; }
        public static int MaxPlayers { get; set; }

        public static void LoadSettings()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(@"C:\\Users\\nikos\\Documents\\Visual Studio Projects\\ServerYourWorldMMORPG\\appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var serverSettings = configuration.GetSection("ServerSettings");

            //Console.WriteLine($"Current working directory: {Environment.CurrentDirectory}");

            IpAddress = serverSettings["IpAddress"];
            TcpPort = int.Parse(serverSettings["TcpPort"]);
            UdpPort = int.Parse(serverSettings["UdpPort"]);
            MaxPlayers = int.Parse(serverSettings["MaxPlayers"]);

            //object[] array = { ipAddress, tcpPort, udpPort, maxPlayers };
            //ConsoleUtility.DebugPrint(array);

            //return new ServerSettings
            //{
            //    IpAddress = ipAddress,
            //    TcpPort = tcpPort,
            //    UdpPort = udpPort,
            //    MaxPlayers = maxPlayers
            //};
        }
    }
}
