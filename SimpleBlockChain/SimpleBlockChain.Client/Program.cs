using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Helpers;
using System;

namespace SimpleBlockChain.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==== Welcome to SimpleBlockChain (FULL-NODE / MINER) ====");
            var network = MenuHelper.ChooseNetwork();
            var launcher = new NodeLauncher(network, ServiceFlags.NODE_NETWORK);
            launcher.StartNodeEvent += StartNodeEvent;
            launcher.NewMessageEvent += NewMessageEvent;
            launcher.ConnectPeerEvent += ConnectPeerEvent;
            launcher.Launch();
        }

        private static void StartNodeEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayInformation("Node is listening");
        }

        private static void NewMessageEvent(object sender, StringEventArgs e)
        {
            MenuHelper.DisplayInformation($"Message {e.Data} arrived");
        }

        private static void ConnectPeerEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayInformation($"Connected to a P2P FULL NODE");
        }
    }
}
