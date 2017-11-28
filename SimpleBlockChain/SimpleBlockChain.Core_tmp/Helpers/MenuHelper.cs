using System;
using System.Text;

namespace SimpleBlockChain.Core.Helpers
{
    public class MenuHelper
    {
        private const string _menuItemSeparator = "----";

        public static Networks ChooseNetwork()
        {
            DisplayMenuItem("Choose on which network you want to connect");
            DisplayMenuItem("1. MainNet", 2);
            DisplayMenuItem("2. TestNet", 2);
            DisplayMenuItem("3. RegTest", 2);
            var number = EnterNumber();
            switch (number)
            {
                case 1:
                    return Networks.MainNet;
                case 2:
                    return Networks.TestNet;
                case 3:
                    return Networks.RegTest;
            }

            DisplayError("Please enter a correct number [1 - 3]");
            return ChooseNetwork();
        }

        public static int EnterNumber()
        {
            int option;
            if (!int.TryParse(Console.ReadLine(), out option))
            {
                DisplayError("Please enter a correct number");
                return EnterNumber();
            }

            return option;
        }

        public static void DisplayInformation(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void DisplayMenuItem(string message, int level = 1)
        {
            var separatorBuilder = new StringBuilder();
            for (var i = 0; i < level; i++)
            {
                separatorBuilder.Append(_menuItemSeparator);
            }

            Console.WriteLine($"{separatorBuilder.ToString()} {message}");
        }
    }
}
