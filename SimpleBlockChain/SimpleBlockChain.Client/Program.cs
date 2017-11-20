using SimpleBlockChain.Core;
using System;

namespace SimpleBlockChain.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==== Welcome to SimpleBlockChain ====");
            var launcher = new NodeLauncher();
            launcher.Launch();
            Console.ReadLine();
            launcher.Dispose();
        }
    }
}
