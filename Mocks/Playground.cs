//using System;
//using System.Threading.Tasks;

namespace ServerYourWorldMMORPG.Mocks
{
//    class PlayYourPipi
//    {
//        static async Task Main(string[] args)
//        {
//            var gameServer = new GameServerYOLO();

//            var player1 = new Player("Alice");
//            var player2 = new Player("Bob");

//            gameServer.AddPlayer(player1);
//            gameServer.AddPlayer(player2);

//            await gameServer.Start();

//            Console.WriteLine("Game server has started.");
//        }
//    }

//    class GameServerYOLO
//    {
//        private List<Player> players = new List<Player>();

//        public void AddPlayer(Player player)
//        {
//            players.Add(player);
//        }

//        public async Task Start()
//        {
//            Console.WriteLine("Starting the game server...");

//            var tasks = new List<Task>();
//            foreach (var player in players)
//            {
//                tasks.Add(player.JoinGame());
//            }

//            await Task.WhenAll(tasks);
//            //await Task.WhenAny(tasks);

//            Console.WriteLine("All players have joined the game.");
//        }
//    }

//    class Player
//    {
//        public string Name { get; }

//        public Player(string name)
//        {
//            Name = name;
//        }

//        public async Task JoinGame()
//        {
//            Random rnd = new Random();
//            Console.WriteLine($"{Name} is joining the game.");
//            await Task.Delay(rnd.Next(1000, 10000));
//            Console.WriteLine($"{Name} has joined the game.");
//        }
//    }
}
