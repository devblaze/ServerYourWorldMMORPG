using System.Drawing;
using Console = Colorful.Console;

namespace ServerYourWorldMMORPG.Utils
{
    public static class ConsoleUtility
    {
        /// <summary>
        /// Print in the console with timestamp and priority color if needed.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="priority"></param>
        public static void Print(string message, int priority = 0)
        {
            string timestamp = GetNowTimestamp();
            string formattedMessage = $"{timestamp} - {FirstLetterCapital(message)}";
            if (priority == 0)
            {
                Console.WriteLine(formattedMessage);
            }
            else
            {
                Console.WriteLine(formattedMessage, PriorityColoredMessage(priority));
            }
        }

        /// <summary>
        /// With this function you can pass a lot of variables in an array and print them in the console.
        /// </summary>
        /// <param name="variables"></param>
        public static void DebugPrint(object[] variables)
        {
            foreach (var variable in variables)
            {
                Console.WriteLine(nameof(variable) + " = " + variable.ToString());
            }
        }

        public static void ClientPrint(string message, int priority = 0)
        {
            Console.Write("Client: ");
            Print(message, priority);
        }

        private static string FirstLetterCapital(string message)
        {
            if (message.Length == 0) return message;

            return char.ToUpper(message[0]).ToString() + message.Substring(1);
        }

        private static Color PriorityColoredMessage(int priority)
        {
            if (priority == 1) return Color.Red; //For some reason this is Blue, don't ask why.
            if (priority == 2) return Color.Yellow; // Green
            if (priority == 3) return Color.OrangeRed;
            if (priority == 4) return Color.Green;

            return Color.White;
        }

        private static string GetNowTimestamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
