using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Nodes;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Validators;

namespace SimpleBlockChain.Core.Factories
{
    public interface IRpcNodeFactory
    {
        RPCNode Build(Networks network);
    }

    internal class RpcNodeFactory : IRpcNodeFactory
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IBlockChainStore _blockChainStore;
        private readonly ITransactionHelper _transactionHelper;
        private readonly ITransactionValidator _transactionValidator;
        private readonly IBlockValidator _blockValidator;

        public RpcNodeFactory(IWalletRepository walletRepository, IBlockChainStore blockChainStore, ITransactionHelper transactionHelper, ITransactionValidator transactionValidator, IBlockValidator blockValidator)
        {
            _walletRepository = walletRepository;
            _blockChainStore = blockChainStore;
            _transactionHelper = transactionHelper;
            _transactionValidator = transactionValidator;
            _blockValidator = blockValidator;
        }

        public RPCNode Build(Networks network)
        {
            return new RPCNode(_walletRepository, network, _blockChainStore, _transactionHelper, _transactionValidator, _blockValidator);
        }
    }
}
