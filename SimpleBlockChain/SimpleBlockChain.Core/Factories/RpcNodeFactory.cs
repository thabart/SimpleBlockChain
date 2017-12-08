using SimpleBlockChain.Core.Nodes;
using SimpleBlockChain.Core.Repositories;

namespace SimpleBlockChain.Core.Factories
{
    public interface IRpcNodeFactory
    {
        RPCNode Build(Networks network);
    }

    internal class RpcNodeFactory : IRpcNodeFactory
    {
        private readonly IWalletRepository _walletRepository;

        public RpcNodeFactory(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public RPCNode Build(Networks network)
        {
            return new RPCNode(_walletRepository, network);
        }
    }
}
