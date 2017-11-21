using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Launchers;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Parsers;
using SimpleBlockChain.Core.States;
using SimpleBlockChain.Core.Storages;
using SimpleBlockChain.Interop;
using System;
using System.Linq;
using System.Net;

namespace SimpleBlockChain.Core.Connectors
{
    public class PeerConnector : IDisposable
    {
        private readonly Networks _network;
        private RpcClientApi _client;
        private MessageParser _messageParser;
        private MessageLauncher _messageLauncher;
        private PeerConnection _peerConnection;

        public PeerConnector(Networks network)
        {
            _client = null;
            _network = network;
            _messageParser = new MessageParser();
            _messageLauncher = new MessageLauncher();
        }

        public void Connect(string host, ServiceFlags serviceFlag)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException(nameof(host));
            }

            var iid = Interop.Constants.InterfaceId;
            var port = PortsHelper.GetPort(_network);
            _client = new RpcClientApi(iid, RpcProtseq.ncacn_ip_tcp, host, port);
            // Connection to peers : https://bitcoin.org/en/developer-guide#connecting-to-peers
            var instance = ConfigurationStorage.Instance();
            var transmittingNode = instance.GetMyIpAddress();
            IPAddress ipAdr = null;
            if (!IPAddress.TryParse(host, out ipAdr))
            {
                // TODO : Throw an exception.
            }

            var adrBytes = ipAdr.MapToIPv6().GetAddressBytes();
            var receivingNode = new IpAddress(serviceFlag, adrBytes, ushort.Parse(port));
            var nonce = NonceHelper.GetNonce();
            var versionMessage = new VersionMessage(transmittingNode, receivingNode, nonce, string.Empty, 0, false, _network);
            var result = _client.Execute(versionMessage.Serialize());
            _peerConnection = new PeerConnection(adrBytes);
            instance.AddPeerConnection(_peerConnection);
            Parse(result);
        }

        public void Execute(byte[] input)
        {
            _client.Execute(input);
        }

        private void Parse(byte[] payload)
        {
            var message = _messageParser.Parse(payload);
            var instance = ConfigurationStorage.Instance();
            Message response = null;
            if (message.GetCommandName() == Constants.MessageNames.Version)
            {
                var versionMessage = message as VersionMessage;
                response = _messageLauncher.Launch(versionMessage);
            }
            else if (message.GetCommandName() == Constants.MessageNames.Verack)
            {
                Console.WriteLine("Client : Connected to a peer");
                _peerConnection.Connect();
                var addrMessage = new AddrMessage(new CompactSize { Size = 1 }, _network);
                var myIpAddress = instance.GetMyIpAddress();
                var ipAddress = new IpAddress(DateTime.UtcNow, myIpAddress.ServiceFlag, myIpAddress.Ipv6, myIpAddress.Port);
                addrMessage.IpAddresses.Add(ipAddress);
                _client.Execute(addrMessage.Serialize());
                response = new GetAddressMessage(_network);
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
