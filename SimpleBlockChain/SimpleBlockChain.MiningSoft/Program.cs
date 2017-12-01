using SimpleBlockChain.Core.Helpers;
using System;

namespace SimpleBlockChain.MiningSoft
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "MINING SOFT";
            Console.WriteLine("==== Welcome to SimpleBlockChain (MINING SOFT) ====");
            var network = MenuHelper.ChooseNetwork();
            var mineService = new MineService(network);
            mineService.Start();
            Console.ReadLine();
        }
    }
}
