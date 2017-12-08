using SimpleBlockChain.Core.Nodes;

namespace SimpleBlockChain.Core.Factories
{
    public interface INodeLauncherFactory
    {
        NodeLauncher Build(Networks network, ServiceFlags serviceFlag);
    }

    internal class NodeLauncherFactory : INodeLauncherFactory
    {
        private readonly IRpcNodeFactory _rpcNodeFactory;

        public NodeLauncherFactory(IRpcNodeFactory rpcNodeFactory)
        {
            _rpcNodeFactory = rpcNodeFactory;
        }

        public NodeLauncher Build(Networks network, ServiceFlags serviceFlag)
        {
            return new NodeLauncher(network, serviceFlag, _rpcNodeFactory);
        }
    }
}
