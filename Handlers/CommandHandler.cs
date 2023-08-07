public class CommandHandler
{
    private readonly ServerCommands _serverCommands;

    public CommandHandler(ServerCommands serverCommands)
    {
        _serverCommands = serverCommands;
    }

    public void ProcessCommand(string input)
    {
        string[] commandParts = input.Split(' ');
        string command = commandParts[0].ToLower();
        string[] arguments = commandParts.Skip(1).ToArray();

        switch (command)
        {
            case "start":
                _serverCommands.StartServer();
                break;
            case "stop":
                _serverCommands.StopServer();
                break;
            // Handle more commands here
            default:
                Console.WriteLine("Unknown command.");
                break;
        }
    }
}
