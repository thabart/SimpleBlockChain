using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Helpers;
using System;
using System.Net;

namespace SimpleBlockChain.FullNode
{
    class Program
    {
        private static NodeLauncher _nodeLauncher;

        static void Main(string[] args)
        {
            Console.WriteLine("==== Welcome to SimpleBlockChain (FULL NODE / MINER) ====");
            var ipBytes = IPAddress.Parse("127.0.0.1").MapToIPv6().GetAddressBytes();
            var network = MenuHelper.ChooseNetwork();
            _nodeLauncher = new NodeLauncher(network, ServiceFlags.NODE_NETWORK, ipBytes);
            _nodeLauncher.StartNodeEvent += StartNodeEvent;
            _nodeLauncher.NewMessageEvent += NewMessageEvent;
            _nodeLauncher.ConnectP2PEvent += ConnectP2PEvent;
            _nodeLauncher.DisconnectP2PEvent += DisconnectP2PEvent;
            _nodeLauncher.Launch();
            Console.ReadLine();
        }

        private static void ConnectP2PEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayInformation("Connected to P2P network");
        }

        private static void DisconnectP2PEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayError("Cannot connect to P2P network... Retry in 10 seconds");
        }

        private static void StartNodeEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayInformation("Node is listening");
            _nodeLauncher.ConnectP2PNetwork();
        }

        private static void NewMessageEvent(object sender, StringEventArgs e)
        {
            MenuHelper.DisplayInformation($"Message {e.Data} arrived");
        }
    }
}
