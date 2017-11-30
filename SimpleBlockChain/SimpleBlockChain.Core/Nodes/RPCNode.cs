using Microsoft.AspNetCore.Hosting;
using SimpleBlockChain.Core.Helpers;
using System;

namespace SimpleBlockChain.Core.Nodes
{
    public class RPCNode : IDisposable
    {
        private readonly Networks _network;
        private IWebHost _host;

        public RPCNode(Networks network)
        {
            _network = network;
        }

        public void Start()
        {
            _host = new WebHostBuilder().UseKestrel()
                .UseUrls($"http://localhost:{PortsHelper.GetRPCPort(_network)}")
                .UseStartup(typeof(RPCNodeStartup))
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
