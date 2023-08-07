using System;
using System.Drawing;
using Console = Colorful.Console;

namespace ServerYourWorldMMORPG.Models.Utils
{
    public static class ConsoleUtility
    {
        public static Color PriorityColoredMessage(int priority)
        {
            if (priority == 1) return Color.Red;
            if (priority == 2) return Color.Yellow;
            if (priority == 3) return Color.OrangeRed;

            return Color.White;
        }

        public static string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static void Print(string message, int priority = 0)
        {
            string timestamp = GetTimestamp();
            string formattedMessage = $"{timestamp} - {message}";
            if (priority == 0)
            {
                Console.WriteLine(formattedMessage);
            } else { 
                Console.WriteLine(formattedMessage, PriorityColoredMessage(priority)); 
            }
        }
    }
}
