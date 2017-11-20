using SimpleBlockChain.Core;
using SimpleBlockChain.Interop;
using System;

namespace SimpleBlockChain.Client
{
    public class PeerConnector : IDisposable
    {
        private readonly Networks _network;
        private RpcClientApi _client;

        public PeerConnector(Networks network)
        {
            _client = null;
            _network = network;
        }

        public void Connect(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException(nameof(host));
            }

            var iid = Interop.Constants.InterfaceId;
            var port = _network == Networks.MainNet ? Core.Constants.Ports.MainNet : Core.Constants.Ports.TestNet;
            _client = new RpcClientApi(iid, RpcProtseq.ncacn_ip_tcp, host, port);

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
