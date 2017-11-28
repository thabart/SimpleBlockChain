using SimpleBlockChain.Core.Connectors;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Launchers;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Parsers;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Interop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SimpleBlockChain.Core
{
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

        public NodeLauncher(Networks network, ServiceFlags serviceFlag, IEnumerable < byte> ipAddress = null)
        {
            _network = network;
            _serviceFlag = serviceFlag;
             _ipAdrHelper = new IpAdrHelper();
            _messageParser = new MessageParser();
            _messageLauncher = new MessageLauncher();
            _p2pNetworkConnector = new P2PNetworkConnector();
            _p2pNetworkConnector.ConnectEvent += P2PConnectEvent;
            _p2pNetworkConnector.DisconnectEvent += P2PDisconnectEvent;
            if (ipAddress == null)
            {
                ipAddress = _ipAdrHelper.GetIpv4Address();
            }

            _ipAddress = new IpAddress(DateTime.UtcNow, _serviceFlag, ipAddress.ToArray(), ushort.Parse(PortsHelper.GetPort(_network)));
        }

        public event EventHandler StartNodeEvent;
        public event EventHandler StopNodeEvent;
        public event EventHandler ConnectP2PEvent;
        public event EventHandler DisconnectP2PEvent;
        public event EventHandler<StringEventArgs> NewMessageEvent;

        public void Launch()
        {
            PeersStore.Instance().SetMyIpAddress(_ipAddress);
            StartNode();
        }

        public void Stop()
        {
            if (_server != null) _server.Dispose();
            _p2pNetworkConnector.Stop();
            if (StopNodeEvent != null)
            {
                StopNodeEvent(this, EventArgs.Empty);
            }
        }

        public Task ConnectP2PNetwork()
        {
            return _p2pNetworkConnector.Listen(_network);
        }

        public bool IsP2PNetworkRunning()
        {
            return _p2pNetworkConnector.IsRunning;
        }

        public ConcurrentBag<PeerConnector> GetActivePeers()
        {
            return _p2pNetworkConnector.GetActivePeers();
        }

        private void StartNode()
        {
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
                if (NewMessageEvent != null)
                {
                    NewMessageEvent(this, new StringEventArgs(message.GetCommandName()));
                }

                Message response = null;
                if (message.GetCommandName() == Constants.MessageNames.Version)
                {
                    var msg = message as VersionMessage;
                    response = _messageLauncher.ServerRespond(msg);
                }
                else if (message.GetCommandName() == Constants.MessageNames.Verack)
                {
                    var verackMessage = message as VerackMessage;
                    response = _messageLauncher.ServerRespond(verackMessage, _network);
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

            if (StartNodeEvent != null)
            {
                StartNodeEvent(this, EventArgs.Empty);
            }
        }

        private void P2PConnectEvent(object sender, EventArgs e)
        {
            if (ConnectP2PEvent != null)
            {
                ConnectP2PEvent(this, EventArgs.Empty);
            }
        }

        private void P2PDisconnectEvent(object sender, EventArgs e)
        {
            if (DisconnectP2PEvent != null)
            {
                DisconnectP2PEvent(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            if (_server != null) _server.Dispose();
            _p2pNetworkConnector.Dispose();
            if (StopNodeEvent != null)
            {
                StopNodeEvent(this, EventArgs.Empty);
            }
        }
    }
}
