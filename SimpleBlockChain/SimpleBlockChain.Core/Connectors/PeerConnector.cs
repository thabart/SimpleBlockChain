using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Launchers;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Parsers;
using SimpleBlockChain.Core.States;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading;

namespace SimpleBlockChain.Core.Connectors
{
    public class PeerConnector : IDisposable
    {
        private const int CHECK_INTERVAL = 11000;
        private readonly BackgroundWorker _cheeckPeerAvailabilityWorker;
        private readonly Networks _network;
        private Timer _timer;
        private RpcClientApi _client;
        private MessageParser _messageParser;
        private MessageLauncher _messageLauncher;
        private PeerConnection _peerConnection;
        private PongMessage _pongMessage;
        private IpAddress _currentIpAddress;
        private ServiceFlags _serviceFlag;
        private AutoResetEvent _autoEvent = null;

        public PeerConnector(Networks network)
        {
            _autoEvent = new AutoResetEvent(false);
            _client = null;
            _network = network;
            _messageParser = new MessageParser();
            _messageLauncher = new MessageLauncher();
            _cheeckPeerAvailabilityWorker = new BackgroundWorker();
            _cheeckPeerAvailabilityWorker.DoWork += CheckPeerAvailability;
        }

        public event EventHandler<IpAddressEventArgs> TimeOutEvent;
        public event EventHandler<IpAddressEventArgs> ConnectEvent;

        public void Connect(string host, ServiceFlags serviceFlag)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException(nameof(host));
            }

            var iid = Interop.Constants.InterfaceId;
            var port = PortsHelper.GetPort(_network);
            IPAddress ipAdr = null;
            if (!IPAddress.TryParse(host, out ipAdr))
            {
                // TODO : Throw an exception.
            }

            var adrBytes = ipAdr.MapToIPv6().GetAddressBytes();
            _serviceFlag = serviceFlag;
            _currentIpAddress = new IpAddress(serviceFlag, adrBytes, ushort.Parse(port));
            // _client = new RpcClientApi(iid, RpcProtseq.ncacn_ip_tcp, host, port);
            _client = new RpcClientApi(iid, RpcProtseq.ncalrpc, null, host);
            // Connection to peers : https://bitcoin.org/en/developer-guide#connecting-to-peers
            var instance = PeersStore.Instance();
            var transmittingNode = instance.GetMyIpAddress();

            var nonce = NonceHelper.GetNonceUInt64();
            var versionMessage = new VersionMessage(transmittingNode, _currentIpAddress, nonce, string.Empty, 0, false, _network);
            try
            {
                var result = _client.Execute(versionMessage.Serialize());
                _peerConnection = new PeerConnection(adrBytes);
                Parse(result);
            }
            catch(RpcException)
            {
                throw new PeerConnectorException(ErrorCodes.PeerRpcError);
            }
        }

        public IpAddress GetCurrentIpAddress()
        {
            return _currentIpAddress;
        }

        public IEnumerable<byte> Execute(byte[] input)
        {
            return _client.Execute(input);
        }

        public PeerConnectionStates GetState()
        {
            return _peerConnection.State;
        }

        public ServiceFlags GetServiceFlag()
        {
            return _serviceFlag;
        }

        private void TimerElapsed(object sender)
        {
            if (!_cheeckPeerAvailabilityWorker.IsBusy)
            {
                _cheeckPeerAvailabilityWorker.RunWorkerAsync();
            }
        }

        private void CheckPeerAvailability(object sender, DoWorkEventArgs e)
        {
            var callback = new Action(() =>
            {
                if (TimeOutEvent != null)
                {
                    TimeOutEvent(this, new IpAddressEventArgs(_currentIpAddress));
                }
            });
            var nonce = NonceHelper.GetNonceUInt64();
            var pingMessage = new PingMessage(nonce, _network);
            try
            {
                var payload = _client.Execute(pingMessage.Serialize());
                var message = _messageParser.Parse(payload);
                if (message.GetCommandName() != Constants.MessageNames.Pong)
                {
                    callback();
                }
                
                var pongMessage = message as PongMessage;
                if (pongMessage == null)
                {
                    callback();
                }

                if (pongMessage.Nonce != nonce)
                {
                    callback();
                }
            }
            catch (RpcException)
            {
                callback();
            }
        }

        private void Parse(byte[] payload)
        {
            var message = _messageParser.Parse(payload);
            var instance = PeersStore.Instance();
            Message response = null;
            if (message.GetCommandName() == Constants.MessageNames.Version)
            {
                var versionMessage = message as VersionMessage;
                response = _messageLauncher.ClientRespond(versionMessage, _network);
            }
            else if (message.GetCommandName() == Constants.MessageNames.Verack)
            {
                _peerConnection.Connect();
                if (ConnectEvent != null)
                {
                    ConnectEvent(this, new IpAddressEventArgs(_currentIpAddress));
                }

                _timer = new Timer(TimerElapsed, _autoEvent, CHECK_INTERVAL, CHECK_INTERVAL); // CHECK PEERS AVAILABILITY EVERY 60 SECONDS.
            }
            else if (message.GetCommandName() == Constants.MessageNames.Pong)
            {
                _pongMessage = message as PongMessage;
            }
            else
            {
                response = _messageLauncher.Launch(message);
            }

            if (response != null)
            {
                Parse(_client.Execute(response.Serialize()));
            }
        }

        public void Disconnect()
        {
            _client.Dispose();
        }

        public void Dispose()
        {
            _client.Dispose();
            _timer.Dispose();
        }
    }
}
