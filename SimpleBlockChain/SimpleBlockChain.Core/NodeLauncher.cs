using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Connectors;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Launchers;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Parsers;
using SimpleBlockChain.Core.Storages;
using SimpleBlockChain.Core.Transactions;
using SimpleBlockChain.Interop;
using System;

namespace SimpleBlockChain.Core
{
    public class NodeLauncher : IDisposable
    {
        private string _serializedHash;
        private BaseTransaction _transaction;
        private static RpcServerApi _server;
        private IpAdrHelper _ipAdrHelper;
        private MessageParser _messageParser;
        private MessageLauncher _messageLauncher;
        private P2PNetworkConnector _p2pNetworkConnector;

        public NodeLauncher()
        {
            _ipAdrHelper = new IpAdrHelper();
            _messageParser = new MessageParser();
            _messageLauncher = new MessageLauncher();
            _p2pNetworkConnector = new P2PNetworkConnector();
        }

        public void Launch()
        {
            var network = ChooseNetwork();
            var serviceFlag = ChooseNodeType();
            ConfigurationStorage.Instance().SetMyIpAddress(new IpAddress(DateTime.UtcNow, serviceFlag, _ipAdrHelper.GetIpv4Address(), ushort.Parse(PortsHelper.GetPort(network))));
            StartNode(network);
            _p2pNetworkConnector.Listen(network);
            DisplayMenu(network);
        }

        private void DisplayMenu(Networks network)
        {
            Console.WriteLine("\t Generate a new address");
            Console.ReadLine();
            _serializedHash = GenerateNewAddress(network);
            Console.WriteLine("\t Create the first transaction");
            Console.ReadLine();
            CreateTransaction(network);
            Console.WriteLine("\t Broadcast the transaction");
            Console.ReadLine();
            BroadCastTransaction();
        }

        private string GenerateNewAddress(Networks network)
        {
            var key = new Key();
            var blockChainAddress = new BlockChainAddress(Transactions.ScriptTypes.P2PKH, network, key);
            var adr = blockChainAddress.GetSerializedHash();
            Console.WriteLine($"\t \t BOB is sending it's address to alice via QR code or another way : {adr}");
            return adr;
        }
        
        private void CreateTransaction(Networks network)
        {

            var receivedBlockChain = BlockChainAddress.Deserialize(_serializedHash);
            var publicKeyHash = receivedBlockChain.PublicKeyHash;
            var transactionBuilder = new TransactionBuilder();
            Console.WriteLine("\t \t Please enter the number of BC to spend");
            var value = EnterNumber();
            var script = Script.CreateP2PKHScript(publicKeyHash);
            transactionBuilder.AddOutput(49, script);
            _transaction = transactionBuilder.Build();
        }

        private void BroadCastTransaction()
        {
            _p2pNetworkConnector.Broadcast(_transaction);
        }

        private void StartNode(Networks network)
        {
            var iid = Interop.Constants.InterfaceId;
            var instance = ConfigurationStorage.Instance();
            _server = new RpcServerApi(iid, 1234, -1, true);
            _server.AddProtocol(RpcProtseq.ncacn_ip_tcp, PortsHelper.GetPort(network), 5);
            _server.StartListening();
            Console.WriteLine("\t The NODE is listening");
            _server.OnExecute += delegate (IRpcClientInfo client, byte[] arg)
            {
                var message = _messageParser.Parse(arg);
                Console.WriteLine(string.Format("\t A message has been received {0}", message.GetCommandName()));
                var peerConnectionLst = instance.GetPeerConnectionLst();
                Message response = null;
                if (message.GetCommandName() == Constants.MessageNames.Version)
                {
                    var msg = message as VersionMessage;
                    response = _messageLauncher.Launch(msg);
                }
                else if (message.GetCommandName() == Constants.MessageNames.Verack)
                {
                    var verackMessage = message as VerackMessage;
                    response = _messageLauncher.Launch(verackMessage, _ipAdrHelper.ParseRpcIpAddress(client));
                    Console.WriteLine("\t Server : Connected to a peer");
                }
                else
                {
                   response = _messageLauncher.Launch(message);
                }

                if (response == null)
                {
                    return new byte[0];
                }

                return response.Serialize();
            };
        }

        private static Networks ChooseNetwork()
        {
            Console.WriteLine("\t Choose on which network you want to connect");
            Console.WriteLine("\t \t 1. MainNet");
            Console.WriteLine("\t \t 2. TestNet");
            Console.WriteLine("\t \t 3. RegTest");
            var number = EnterNumber();
            switch (number)
            {
                case 1:
                    return Networks.MainNet;
                case 2:
                    return Networks.TestNet;
                default:
                    return Networks.RegTest;
            }
        }

        private static ServiceFlags ChooseNodeType()
        {
            Console.WriteLine("\t Choose the type of node");
            Console.WriteLine("\t \t 1. Lightweight node");
            Console.WriteLine("\t \t 2. Full node"); // https://en.bitcoin.it/wiki/Full_node
            var number = EnterNumber();
            switch (number)
            {
                case 1:
                    return ServiceFlags.NODE_NONE;
                default:
                    return ServiceFlags.NODE_NETWORK;
            }
        }

        private static int EnterNumber()
        {
            int option;
            if (!int.TryParse(Console.ReadLine(), out option))
            {
                Console.WriteLine("Please enter a correct number");
                return EnterNumber();
            }

            return option;
        }

        public void Dispose()
        {
            if (_server != null) _server.Dispose();
            _p2pNetworkConnector.Dispose();
        }
    }
}
