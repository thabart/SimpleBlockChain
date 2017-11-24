using SimpleBlockChain.Core.Connectors;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Launchers;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Parsers;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Interop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core
{
    public class StringEventArgs : EventArgs
    {
        public string Data { get; set; }
        public StringEventArgs(string data)
        {
            Data = data;
        }
    }

    public class NodeLauncher : IDisposable
    {
        private readonly Networks _network;
        private readonly ServiceFlags _serviceFlag;
        private readonly IpAddress _ipAddress;
        private static RpcServerApi _server;
        private IpAdrHelper _ipAdrHelper;
        private MessageParser _messageParser;
        private MessageLauncher _messageLauncher;
        private P2PNetworkConnector _p2pNetworkConnector;

        public NodeLauncher(Networks network, ServiceFlags serviceFlag, IEnumerable<byte> ipAddress = null)
        {
            _network = network;
            _serviceFlag = serviceFlag;
            if (ipAddress == null)
            {
                ipAddress = _ipAdrHelper.GetIpv4Address();
            }

            _ipAddress = new IpAddress(DateTime.UtcNow, _serviceFlag, ipAddress.ToArray(), ushort.Parse(PortsHelper.GetPort(_network))); ;
             _ipAdrHelper = new IpAdrHelper();
            _messageParser = new MessageParser();
            _messageLauncher = new MessageLauncher();
            _p2pNetworkConnector = new P2PNetworkConnector();
        }

        public event EventHandler StartNodeEvent;
        public event EventHandler ConnectPeerEvent;
        public event EventHandler<StringEventArgs> NewMessageEvent;

        public void Launch()
        {
            PeersStore.Instance().SetMyIpAddress(_ipAddress);
            StartNode();
        }
        
        public void ConnectP2PNetwork()
        {
            _p2pNetworkConnector.Listen(_network);
        }

        private void StartNode()
        {
            var iid = Interop.Constants.InterfaceId;
            var instance = PeersStore.Instance();
            _server = new RpcServerApi(iid, 1234, -1, true);
            _server.AddProtocol(RpcProtseq.ncacn_ip_tcp, PortsHelper.GetPort(_network), 5);
            _server.StartListening();
            if (StartNodeEvent != null)
            {
                StartNodeEvent(this, EventArgs.Empty);
            }

            _server.OnExecute += delegate (IRpcClientInfo client, byte[] arg)
            {
                var message = _messageParser.Parse(arg);
                if (NewMessageEvent != null)
                {
                    new EventArgs();
                    NewMessageEvent(this, new StringEventArgs(message.GetCommandName()));
                }

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
                    if (ConnectPeerEvent != null)
                    {
                        ConnectPeerEvent(this, EventArgs.Empty);
                    }
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

        public void Dispose()
        {
            if (_server != null) _server.Dispose();
            _p2pNetworkConnector.Dispose();
        }
    }
}
