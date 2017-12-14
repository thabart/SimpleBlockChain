using SimpleBlockChain.Core.Connectors;
using SimpleBlockChain.Core.Nodes;
using SimpleBlockChain.Core.Stores;

namespace SimpleBlockChain.Core.Factories
{
    public interface INodeLauncherFactory
    {
        NodeLauncher Build(Networks network, ServiceFlags serviceFlag);
    }

    internal class NodeLauncherFactory : INodeLauncherFactory
    {
        private readonly IRpcNodeFactory _rpcNodeFactory;
        private readonly IBlockChainStore _blockChainStore;
        private readonly IMessageCoordinator _messageCoordinator;

        public NodeLauncherFactory(IRpcNodeFactory rpcNodeFactory, IBlockChainStore blockChainStore, IMessageCoordinator messageCoordinator)
        {
            _rpcNodeFactory = rpcNodeFactory;
            _blockChainStore = blockChainStore;
            _messageCoordinator = messageCoordinator;
        }

        public NodeLauncher Build(Networks network, ServiceFlags serviceFlag)
        {
            return new NodeLauncher(network, serviceFlag, _rpcNodeFactory, _blockChainStore, _messageCoordinator);
        }
    }
}
