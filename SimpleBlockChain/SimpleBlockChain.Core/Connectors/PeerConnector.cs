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
using System.ComponentModel;
using System.Net;
using System.Timers;

namespace SimpleBlockChain.Core.Connectors
{
    public class PeerConnector : IDisposable
    {
        private const int CHECK_INTERVAL = 60000;
        private const int PEER_TIMEOUT = 11000;
        private readonly BackgroundWorker _cheeckPeerAvailabilityWorker;
        private readonly Networks _network;
        private RpcClientApi _client;
        private MessageParser _messageParser;
        private MessageLauncher _messageLauncher;
        private PeerConnection _peerConnection;
        private PongMessage _pongMessage;
        private IpAddress _currentIpAddress;

        public PeerConnector(Networks network)
        {
            _client = null;
            _network = network;
            _messageParser = new MessageParser();
            _messageLauncher = new MessageLauncher();
            _cheeckPeerAvailabilityWorker = new BackgroundWorker();
            _cheeckPeerAvailabilityWorker.DoWork += CheckPeerAvailability;
            Timer timer = new Timer(CHECK_INTERVAL); // CHECK PEERS AVAILABILITY EVERY 60 SECONDS.
            timer.Elapsed += TimerElapsed;
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
            _client = new RpcClientApi(iid, RpcProtseq.ncacn_ip_tcp, "localhost", port);
            // Connection to peers : https://bitcoin.org/en/developer-guide#connecting-to-peers
            var instance = PeersStore.Instance();
            var transmittingNode = instance.GetMyIpAddress();
            IPAddress ipAdr = null;
            if (!IPAddress.TryParse(host, out ipAdr))
            {
                // TODO : Throw an exception.
            }

            var adrBytes = ipAdr.MapToIPv6().GetAddressBytes();
            var receivingNode = new IpAddress(serviceFlag, adrBytes, ushort.Parse(port));
            var nonce = NonceHelper.GetNonceUInt64();
            var versionMessage = new VersionMessage(transmittingNode, receivingNode, nonce, string.Empty, 0, false, _network);
            try
            {
                var result = _client.Execute(versionMessage.Serialize());
                _peerConnection = new PeerConnection(adrBytes);
                instance.AddPeerConnection(_peerConnection);
                Parse(result);
            }
            catch(RpcException)
            {
                throw new PeerConnectorException(ErrorCodes.PeerRpcError);
            }
        }

        public void Execute(byte[] input)
        {
            _client.Execute(input);
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!_cheeckPeerAvailabilityWorker.IsBusy)
            {
                _cheeckPeerAvailabilityWorker.RunWorkerAsync();
            }
        }
        
        private void CheckPeerAvailability(object sender, DoWorkEventArgs e)
        {
            var nonce = NonceHelper.GetNonceUInt64();
            _messageLauncher.Launch(new PingMessage(nonce, _network));
            int time = 0;
            int intervalMs = 200;
            while (time < PEER_TIMEOUT)
            {
                if (_pongMessage != null && _pongMessage.Nonce == nonce)
                {
                    _pongMessage = null;
                    return;
                }

                System.Threading.Thread.Sleep(intervalMs);
                time += intervalMs;
            }


            if (TimeOutEvent != null)
            {
                TimeOutEvent(this, new IpAddressEventArgs(_currentIpAddress));
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
                response = _messageLauncher.Launch(versionMessage);
            }
            else if (message.GetCommandName() == Constants.MessageNames.Verack)
            {
                _peerConnection.Connect();
                var myIpAddress = instance.GetMyIpAddress();
                _currentIpAddress = new IpAddress(DateTime.UtcNow, myIpAddress.ServiceFlag, myIpAddress.Ipv6, myIpAddress.Port);
                if (ConnectEvent != null)
                {
                    ConnectEvent(this, new IpAddressEventArgs(_currentIpAddress));
                }

                var addrMessage = new AddrMessage(new CompactSize { Size = 1 }, _network);
                addrMessage.IpAddresses.Add(_currentIpAddress);
                _client.Execute(addrMessage.Serialize());
                response = new GetAddressMessage(_network);
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
        }
    }
}
