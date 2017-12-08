using Microsoft.AspNetCore.Hosting;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Repositories;
using System;

namespace SimpleBlockChain.Core.Nodes
{
    public class RPCNode : IDisposable
    {
        private readonly IWalletRepository _walletRepository;
        private readonly Networks _network;
        private IWebHost _host;

        internal RPCNode(IWalletRepository walletRepository, Networks network)
        {
            _walletRepository = walletRepository;
            _network = network;
        }

        public void Start()
        {
            var rpcNodeStartup = new RPCNodeStartup(_walletRepository, _network);
            _host = new WebHostBuilder().UseKestrel()
                .UseUrls($"http://localhost:{PortsHelper.GetRPCPort(_network)}")
                .Configure((app) =>
                {
                    rpcNodeStartup.Configure(app);
                })
                .Build();
            _host.Start();
        }

        public void Dispose()
        {
            if (_host == null) { return; }
            _host.Dispose();
            _host = null;
        }
    }
}
