using SimpleBlockChain.Core.Connectors;
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
        private readonly IBlockChainFactory _blockChainFactory;
        private readonly IMessageCoordinator _messageCoordinator;

        public NodeLauncherFactory(IRpcNodeFactory rpcNodeFactory, IBlockChainFactory blockChainFactory, IMessageCoordinator messageCoordinator)
        {
            _rpcNodeFactory = rpcNodeFactory;
            _blockChainFactory = blockChainFactory;
            _messageCoordinator = messageCoordinator;
        }

        public NodeLauncher Build(Networks network, ServiceFlags serviceFlag)
        {
            return new NodeLauncher(network, serviceFlag, _rpcNodeFactory, _blockChainFactory, _messageCoordinator);
        }
    }
}
