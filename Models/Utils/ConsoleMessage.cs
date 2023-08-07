using System;
using System.Drawing;
using Console = Colorful.Console;

namespace ServerYourWorldMMORPG.Models.Utils
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
            string formattedMessage = $"{timestamp} - {message}";
            if (priority == 0)
            {
                Console.WriteLine(formattedMessage);
            } else { 
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

        private static Color PriorityColoredMessage(int priority)
        {
            if (priority == 1) return Color.Red;
            if (priority == 2) return Color.Yellow;
            if (priority == 3) return Color.OrangeRed;

            return Color.White;
        }

        private static string GetNowTimestamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
