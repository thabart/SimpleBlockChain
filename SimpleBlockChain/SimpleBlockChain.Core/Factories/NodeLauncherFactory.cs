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
        private readonly ISmartContractStore _smartContractStore;

        public NodeLauncherFactory(IRpcNodeFactory rpcNodeFactory, IBlockChainStore blockChainStore, IMessageCoordinator messageCoordinator,  ISmartContractStore smartContractStore)
        {
            _rpcNodeFactory = rpcNodeFactory;
            _blockChainStore = blockChainStore;
            _messageCoordinator = messageCoordinator;
            _smartContractStore = smartContractStore;
        }

        public NodeLauncher Build(Networks network, ServiceFlags serviceFlag)
        {
            return new NodeLauncher(network, serviceFlag, _rpcNodeFactory, _blockChainStore, _messageCoordinator, _smartContractStore);
        }
    }
}
