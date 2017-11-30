using SimpleBlockChain.Core.Connectors;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Launchers;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Parsers;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SimpleBlockChain.Core.Nodes
{
    public class P2PNode : IDisposable
    {
        private readonly Networks _network;
        private readonly ServiceFlags _serviceFlag;
        private readonly P2PNetworkConnector _p2pNetworkConnector;
        private static RpcServerApi _server;
        private readonly MessageParser _messageParser;
        private readonly MessageCoordinator _messageCoordinator;
        private IpAdrHelper _ipAdrHelper;
        private IpAddress _ipAddress;

        public P2PNode(Networks network, ServiceFlags serviceFlag, P2PNetworkConnector p2pNetworkConnector)
        {
            _network = network;
            _serviceFlag = serviceFlag;
            _p2pNetworkConnector = p2pNetworkConnector;
            _messageParser = new MessageParser();
            _messageCoordinator = new MessageCoordinator();
            _ipAdrHelper = new IpAdrHelper();
        }

        public event EventHandler StartNodeEvent;
        public event EventHandler StopNodeEvent;
        public event EventHandler<StringEventArgs> NewMessageEvent;

        public void Start(IEnumerable<byte> ipAddress = null)
        {
            if (ipAddress == null)
            {
                ipAddress = _ipAdrHelper.GetIpv4Address();
            }

            _ipAddress = new IpAddress(DateTime.UtcNow, _serviceFlag, ipAddress.ToArray(), ushort.Parse(PortsHelper.GetPort(_network)));
            PeersStore.Instance().SetMyIpAddress(_ipAddress);
            var iid = Interop.Constants.InterfaceId;
            var instance = PeersStore.Instance();
            var port = PortsHelper.GetPort(_network);
            // _server = new RpcServerApi(iid, 1234, -1, true);
            // _server.AddProtocol(RpcProtseq.ncacn_ip_tcp, port, 5);
            var name = (new IPAddress(_ipAddress.Ipv6)).MapToIPv4().ToString(); // USE QUEUE.
            _server = new RpcServerApi(iid, 1234, -1, true);
            _server.AddProtocol(RpcProtseq.ncalrpc, name, 5);
            _server.StartListening();
            _server.OnExecute += delegate (IRpcClientInfo client, byte[] arg)
            {
                var message = _messageParser.Parse(arg);
                var connectedPeer = _p2pNetworkConnector.GetPeer(message.MessageHeader.Ipv6);
                if (NewMessageEvent != null)
                {
                    NewMessageEvent(this, new StringEventArgs(message.GetCommandName()));
                }

                var response = _messageCoordinator.Receive(message, connectedPeer, _p2pNetworkConnector);
                if (response == null)
                {
                    return new byte[0];
                }

                return response.Serialize();
            };

            if (StartNodeEvent != null)
            {
                StartNodeEvent(this, EventArgs.Empty);
            }
        }

        public void Stop()
        {
            if (_server == null) { return; }
            _server.Dispose();
            _server = null;
            if (StopNodeEvent != null)
            {
                StopNodeEvent(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            if (_server == null) { return; }
            _server.Dispose();
            if (StopNodeEvent != null)
            {
                StopNodeEvent(this, EventArgs.Empty);
            }
        }
    }
}
