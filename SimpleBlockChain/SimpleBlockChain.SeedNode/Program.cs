using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Nodes;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Net;

namespace SimpleBlockChain.Client
{
    class Program
    {
        private static NodeLauncher _nodeLauncher;

        static void Main(string[] args)
        {
            Console.WriteLine("==== Welcome to SimpleBlockChain (SEED NODE) ====");
            var network = MenuHelper.ChooseNetwork();
            var ipBytes = IPAddress.Parse(Constants.DNS_IP_ADDRESS).MapToIPv6().GetAddressBytes();
            _nodeLauncher = new NodeLauncher(network, ServiceFlags.NODE_NETWORK);
            var p2pNode = _nodeLauncher.GetP2PNode();
            p2pNode.StartNodeEvent += StartP2PNodeEvent;
            p2pNode.NewMessageEvent += NewP2PMessageEvent;
            _nodeLauncher.ConnectP2PEvent += ConnectP2PEvent;
            _nodeLauncher.DisconnectP2PEvent += DisconnectP2PEvent;
            _nodeLauncher.LaunchP2PNode(ipBytes);
            Console.ReadLine();
            /*
            var ba = BuildBlockChainAddress(); // ADD FAKE TRANSACTION TO MEMORY POOL.
            var builder = new TransactionBuilder();
            var transaction = builder.NewNoneCoinbaseTransaction()
                 .AddOutput(20, Script.CreateP2PKHScript(ba.PublicKeyHash))
                 .Build();
            MemoryPool.Instance().AddTransaction(transaction);
            */
        }

        private static void ConnectP2PEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayInformation("Connected to P2P network");
        }

        private static void DisconnectP2PEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayError("Cannot connect to P2P network... Retry in 10 seconds");
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

        private static BlockChainAddress BuildBlockChainAddress()
        {
            var network = Networks.MainNet;
            var key = Key.Genererate();
            var blockChainAddress = new BlockChainAddress(ScriptTypes.P2PKH, network, key);
            var hash = blockChainAddress.GetSerializedHash();
            var deserializedBA = BlockChainAddress.Deserialize(hash);
            return deserializedBA;
        }
    }
}
