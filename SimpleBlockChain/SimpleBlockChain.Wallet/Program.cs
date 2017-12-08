using Microsoft.Extensions.DependencyInjection;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Nodes;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using SimpleBlockChain.Data.Sqlite;
using System;
using System.Net;
using System.Numerics;

namespace SimpleBlockChain.Wallet
{
    class Program
    {
        private static RpcClient _rpcClient;
        private static NodeLauncher _nodeLauncher;
        private static KeyRepository _keyRepository = new KeyRepository();

        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCore();
            serviceCollection.AddInMemory();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var nodeLauncherFactory = serviceProvider.GetService<INodeLauncherFactory>();

            // https://bitcoin.org/en/developer-guide#full-service-wallets
            Console.Title = "WALLET NODE";
            Console.WriteLine("==== Welcome to SimpleBlockChain (WALLET) ====");
            var network = MenuHelper.ChooseNetwork();
            _rpcClient = new RpcClient(network);
            var ipBytes = IPAddress.Parse("192.168.76.132").MapToIPv6().GetAddressBytes(); // VIRTUAL NETWORK.
            _nodeLauncher = nodeLauncherFactory.Build(network, ServiceFlags.NODE_NONE);
            var p2pNode = _nodeLauncher.GetP2PNode();
            p2pNode.StartNodeEvent += StartP2PNodeEvent;
            p2pNode.NewMessageEvent += NewP2PMessageEvent;
            _nodeLauncher.ConnectP2PEvent += ConnectP2PEvent;
            _nodeLauncher.DisconnectP2PEvent += DisconnectP2PEvent;
            _nodeLauncher.LaunchP2PNode(ipBytes);
            ExecuteMenu();
        }

        private static void DisplayMenu()
        {
            Console.WriteLine("What-do you want to do ?");
            var isP2PNetworkRunning = _nodeLauncher.IsP2PNetworkRunning();
            if (isP2PNetworkRunning)
            {
                DisplayConnectedWallet();
            }
            else
            {
                DisplayDisconnectedWallet();
            }
        }

        private static void ExecuteMenu()
        {
            DisplayMenu();
            var number = MenuHelper.EnterNumber();
            var isP2PNetworkRunning = _nodeLauncher.IsP2PNetworkRunning();
            if (isP2PNetworkRunning)
            {
                ExecuteConnectedWallet(number);
            }
            else
            {
                ExecuteDisconnectedWallet(number);
            }
        }

        private static void DisplayConnectedWallet()
        {
            MenuHelper.DisplayMenuItem("1. Send a transaction");
            MenuHelper.DisplayMenuItem("2. Receive money");
            MenuHelper.DisplayMenuItem("3. See my amount of bitcoins");
            MenuHelper.DisplayMenuItem("4. Refresh the BlockChain");
            MenuHelper.DisplayMenuItem("5. Refresh the connected peers");
            MenuHelper.DisplayMenuItem("6. Exit the application");
        }

        private static void ExecuteConnectedWallet(int number)
        {
            if (number < 0 && number > 6)
            {
                MenuHelper.DisplayError("Please enter an option between [1-6]");
            }
            switch (number)
            {
                case 1: // BROADCAST A UTXO TRANSACTION.
                    Console.WriteLine("Please enter the address");
                    var receivedHash = Console.ReadLine();
                    var deserializedAdr = BlockChainAddress.Deserialize(receivedHash);
                    Console.WriteLine("How much do-you want to send ?");
                    var value = MenuHelper.EnterNumber();
                    var blockChain = BlockChainStore.Instance().GetBlockChain();
                    var builder = new TransactionBuilder();
                    var transaction = builder.NewNoneCoinbaseTransaction()
                         // .Spend(0, )
                         .AddOutput(value, Script.CreateP2PKHScript(deserializedAdr.PublicKeyHash))
                         .Build();
                    var serializedTransaction = transaction.Serialize(); // SEND UTXO.
                    _nodeLauncher.Broadcast(transaction);
                    ExecuteMenu();
                    return;
                case 2:  // GENERATE A NEW BITCOIN ADDRESS.
                    var key = Key.Genererate();
                    var h = new BigInteger(key.GetPublicKeyHashed());
                    var blockChainAddress = new BlockChainAddress(ScriptTypes.P2PKH, _nodeLauncher.GetNetwork(), key);
                    var hash = blockChainAddress.GetSerializedHash();
                    Console.WriteLine($"Give the bitcoin address to the person {hash}");
                    Console.WriteLine("Please enter a password to protect your wallet");
                    var password = Console.ReadLine();
                    _keyRepository.Load(password);
                    _keyRepository.Keys.Add(key);
                    _keyRepository.Save(password);
                    break;
                case 3:
                    DisplayWalletInformation();
                    ExecuteMenu();
                    return;
                case 4:
                    _nodeLauncher.RefreshBlockChain();
                    ExecuteMenu();
                    break;
                case 5:
                    _nodeLauncher.RefreshConnectedPeers();
                    ExecuteMenu();
                    break;
                case 6:
                    Console.WriteLine("Bye bye");
                    Console.ReadLine();
                    return;
            }

            ExecuteMenu();
        }

        private static void DisplayDisconnectedWallet()
        {
            MenuHelper.DisplayMenuItem("1. Exit the application");
        }

        private static void ExecuteDisconnectedWallet(int number)
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

        private static void DisplayWalletInformation()
        {

        }
    }
}
