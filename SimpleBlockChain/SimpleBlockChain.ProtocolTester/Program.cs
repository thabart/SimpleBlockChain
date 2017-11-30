using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Messages.DataMessages;
using SimpleBlockChain.Core.Nodes;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Net;

namespace SimpleBlockChain.ProtocolTester
{
    class Program
    {
        private static RpcClient _rpcClient;
        private static NodeLauncher _nodeLauncher;
        private static KeyRepository _keyRepository = new KeyRepository();

        static void Main(string[] args)
        {
            // https://bitcoin.org/en/developer-guide#full-service-wallets
            Console.Title = "PROTOCOL TESTER NODE";
            Console.WriteLine("==== Welcome to SimpleBlockChain (PROTOCOL TESTER) ====");
            var network = MenuHelper.ChooseNetwork();
            _rpcClient = new RpcClient(network);
            var ipBytes = IPAddress.Parse("192.168.76.133").MapToIPv6().GetAddressBytes(); // VIRTUAL NETWORK.
            _nodeLauncher = new NodeLauncher(network, ServiceFlags.NODE_NONE);
            var p2pNode = _nodeLauncher.GetP2PNode();
            p2pNode.StartNodeEvent += StartP2PNodeEvent;
            p2pNode.NewMessageEvent += NewP2PMessageEvent;
            _nodeLauncher.ConnectP2PEvent += ConnectP2PEvent;
            _nodeLauncher.DisconnectP2PEvent += DisconnectP2PEvent;
            _nodeLauncher.LaunchP2PNode(ipBytes);
            _nodeLauncher.LaunchRPCNode();
            ExecuteMenu();
        }

        private static void DisplayMenu()
        {
            Console.WriteLine("What-do you want to do ?");
            var isP2PNetworkRunning = _nodeLauncher.IsP2PNetworkRunning();
            if (isP2PNetworkRunning)
            {
                DisplayConnected();
            }
            else
            {
                DisplayDisconnected();
            }
        }

        private static void ExecuteMenu()
        {
            DisplayMenu();
            var number = MenuHelper.EnterNumber();
            var isP2PNetworkRunning = _nodeLauncher.IsP2PNetworkRunning();
            if (isP2PNetworkRunning)
            {
                ExecuteConnected(number);
            }
            else
            {
                ExecuteDisconnected(number);
            }
        }

        private static void DisplayConnected()
        {
            MenuHelper.DisplayMenuItem("1. getrawmempool");
            MenuHelper.DisplayMenuItem("2. mempool");
            MenuHelper.DisplayMenuItem("3. Exit the application");
        }

        private static void ExecuteConnected(int number)
        {
            if (number < 0 && number > 2)
            {
                MenuHelper.DisplayError("Please enter an option between [1-3]");
            }
            switch (number)
            {
                case 1: // getrawmempool.
                    var result = _rpcClient.GetRawMemPool().Result;
                    Console.WriteLine(result);
                    ExecuteMenu();
                    return;
                case 2: // mempool.
                    _nodeLauncher.Broadcast(new MemPoolMessage(_nodeLauncher.GetNetwork()));
                    break;
                case 3:
                    Console.WriteLine("Bye bye");
                    Console.ReadLine();
                    return;
            }

            ExecuteMenu();
        }

        private static void DisplayDisconnected()
        {
            MenuHelper.DisplayMenuItem("1. Exit the application");
        }

        private static void ExecuteDisconnected(int number)
        {
            if (number < 0 && number > 1)
            {
                MenuHelper.DisplayError("Please enter an option between [1-1]");
            }

            switch(number)
            {
                case 1:
                    Console.WriteLine("Bye bye");
                    Console.ReadLine();
                    return;
            }

            ExecuteMenu();
        }

        private static void ConnectP2PEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayInformation("Connected to P2P network");
            DisplayMenu();
        }

        private static void DisconnectP2PEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayError("Cannot connect to P2P network... Retry in 10 seconds");
            DisplayMenu();
        }

        private static void StartP2PNodeEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayInformation("Node is listening");
            _nodeLauncher.ConnectP2PNetwork();
        }

        private static void NewP2PMessageEvent(object sender, StringEventArgs e)
        {
            MenuHelper.DisplayInformation($"Message {e.Data} arrived");
        }
    }
}
