using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Nodes;
using System;
using System.Linq;
using System.Net;
using System.Threading;

namespace SimpleBlockChain.FullNode
{
    class Program
    {
        private static NodeLauncher _nodeLauncher;
        private static byte[] _ipBytes;

        static void Main(string[] args)
        {
            Console.Title = "FULL NODE / MINER";
            Console.WriteLine("==== Welcome to SimpleBlockChain (FULL NODE / MINER) ====");
            _ipBytes = IPAddress.Parse("127.0.0.1").MapToIPv6().GetAddressBytes();
            var network = MenuHelper.ChooseNetwork();
            _nodeLauncher = new NodeLauncher(network, ServiceFlags.NODE_NETWORK);
            var p2pNode = _nodeLauncher.GetP2PNode();
            p2pNode.StartNodeEvent += StartP2PNodeEvent;
            p2pNode.StopNodeEvent += StopP2PNodeEvent;
            p2pNode.NewMessageEvent += NewMessageP2PEvent;
            _nodeLauncher.ConnectP2PEvent += ConnectP2PEvent;
            _nodeLauncher.DisconnectP2PEvent += DisconnectP2PEvent;
            _nodeLauncher.LaunchP2PNode(_ipBytes);
            ExecuteFullNodeMenu();
        }

        public static void DisplayFullNodeMenu()
        {
            MenuHelper.DisplayMenuItem("What-do you want to do ?");
            var isP2PNetworkRunning = _nodeLauncher.IsP2PNetworkRunning();
            if (isP2PNetworkRunning)
            {
                DisplayP2PNetworkRunningMenu();
            }
            else
            {
                DisplayP2PNetworkNotRunningMenu();
            }
        }

        public static void ExecuteFullNodeMenu()
        {
            DisplayFullNodeMenu();
            var number = MenuHelper.EnterNumber();
            var isP2PNetworkRunning = _nodeLauncher.IsP2PNetworkRunning();
            if (isP2PNetworkRunning)
            {
                ExecuteP2PNetworkRunningMenu(number);
            }
            else
            {
                ExecuteP2PNetworkNotRunningMenu(number);
            }
        }

        private static void ExecuteP2PNetworkNotRunningMenu(int number)
        {
            if (number < 0 || number > 1)
            {
                MenuHelper.DisplayError("Enter a number between [1-1]");
                ExecuteFullNodeMenu();
                return;
            }

            switch(number)
            {
                case 1:
                    _nodeLauncher.LaunchP2PNode(_ipBytes);
                    break;
            }

            ExecuteFullNodeMenu();
        }

        private static void DisplayP2PNetworkNotRunningMenu()
        {
            MenuHelper.DisplayMenuItem("1. Start the node");
        }

        private static void ExecuteP2PNetworkRunningMenu(int number)
        {
            if (number < 0 || number > 3)
            {
                MenuHelper.DisplayError("Enter a number between [1-3]");
                ExecuteFullNodeMenu();
                return;
            }

            switch (number)
            {
                case 1:
                    Console.WriteLine($"Number of active nodes {_nodeLauncher.GetActivePeers().Count}");
                    break;
                case 2:
                    var peers = _nodeLauncher.GetActivePeers();
                    var ips = peers.Select(p => (new IPAddress(p.GetCurrentIpAddress().Ipv6)).MapToIPv4().ToString());
                    foreach(var ip in ips)
                    {
                        Console.WriteLine(ip);
                    }
                    break;
                case 3:
                    _nodeLauncher.GetP2PNode().Stop();
                    break;
            }

            ExecuteFullNodeMenu();
        }

        private static void DisplayP2PNetworkRunningMenu()
        {
            MenuHelper.DisplayMenuItem("1. Display number of FULL NODES", 1);
            MenuHelper.DisplayMenuItem("2. Display active peers", 1);
            MenuHelper.DisplayMenuItem("3. Stop the node", 1);
        }

        private static void ConnectP2PEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayInformation("Connected to P2P network");
            DisplayFullNodeMenu();
        }

        private static void DisconnectP2PEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayError("Cannot connect to P2P network... Retry in 10 seconds");
        }

        private static void StartP2PNodeEvent(object sender, EventArgs e)
        {
            // MenuHelper.DisplayInformation("Node is listening");
            _nodeLauncher.ConnectP2PNetwork();
        }

        private static void StopP2PNodeEvent(object sender, EventArgs e)
        {
            // MenuHelper.DisplayInformation("Node is not listening");
            DisplayFullNodeMenu();
        }

        private static void NewMessageP2PEvent(object sender, StringEventArgs e)
        {
            // MenuHelper.DisplayInformation($"Message {e.Data} arrived");
        }
    }
}
