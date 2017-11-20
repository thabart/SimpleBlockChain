using SimpleBlockChain.Core.Connectors;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Launchers;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Parsers;
using SimpleBlockChain.Core.Storages;
using SimpleBlockChain.Interop;
using System;
using System.Net;

namespace SimpleBlockChain.Core
{
    public class NodeLauncher : IDisposable
    {
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
                    _messageLauncher.Launch(verackMessage, _ipAdrHelper.ParseRpcIpAddress(client));
                    return new byte[0];
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
