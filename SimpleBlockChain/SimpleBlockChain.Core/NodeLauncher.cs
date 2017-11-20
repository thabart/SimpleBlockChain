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
            Console.WriteLine("Generate a new address");
            Console.ReadLine();
            _serializedHash = GenerateNewAddress(network);
            Console.WriteLine("Create the first transaction & broadcast it");
            Console.ReadLine();
            CreateTransaction(network);
        }

        private string GenerateNewAddress(Networks network)
        {
            var key = new Key();
            var blockChainAddress = new BlockChainAddress(Transactions.ScriptTypes.P2PKH, network, key);
            var adr = blockChainAddress.GetSerializedHash();
            Console.WriteLine($"BOB is sending it's address to alice via QR code or another way : {adr}");
            return adr;
        }
        
        private void CreateTransaction(Networks network)
        {

            var receivedBlockChain = BlockChainAddress.Deserialize(_serializedHash);
            var publicKeyHash = receivedBlockChain.PublicKeyHash;
            var transactionBuilder = new TransactionBuilder();
            Console.WriteLine("Please enter the number of BC to spend");
            var value = EnterNumber();
            var transaction = transactionBuilder.Build();
            var serializedTransaction = transaction.Serialize();
            Console.WriteLine("ALICE is parsing the address & create the first transaction & broadcast it to the network");
        }

        private void StartNode(Networks network)
        {
            var iid = Interop.Constants.InterfaceId;
            var instance = ConfigurationStorage.Instance();
            _server = new RpcServerApi(iid, 1234, -1, true);
            _server.AddProtocol(RpcProtseq.ncacn_ip_tcp, PortsHelper.GetPort(network), 5);
            _server.StartListening();
            Console.WriteLine("The NODE is listening");
            _server.OnExecute += delegate (IRpcClientInfo client, byte[] arg)
            {
                var message = _messageParser.Parse(arg);
                Console.WriteLine(string.Format("A message has been received {0}", message.GetCommandName()));
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
                    Console.WriteLine("Server : Connected to a peer");
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
            Console.WriteLine("Choose on which network you want to connect");
            Console.WriteLine("1. MainNet");
            Console.WriteLine("2. TestNet");
            Console.WriteLine("3. RegTest");
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
            Console.WriteLine("Choose the type of node");
            Console.WriteLine("1. Lightweight node");
            Console.WriteLine("2. Full node"); // https://en.bitcoin.it/wiki/Full_node
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
