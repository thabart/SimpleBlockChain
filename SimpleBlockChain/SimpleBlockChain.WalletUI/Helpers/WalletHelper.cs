using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Aggregates;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;

namespace SimpleBlockChain.WalletUI.Helpers
{
    public interface IWalletHelper
    {
        Key CreateNewAddress();
    }

    public class WalletHelper : IWalletHelper
    {
        private readonly IWalletRepository _walletRepository;

        public WalletHelper(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public Key CreateNewAddress()
        {
            var walletStore = WalletStore.Instance();
            var authenticatedWallet = walletStore.GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                return null;
            }

            var key = Key.Genererate();
            var blockChainAdr = new BlockChainAddress(ScriptTypes.P2PKH, authenticatedWallet.Network, key);
            authenticatedWallet.Addresses.Add(new WalletAggregateAddress
            {
                Hash = blockChainAdr.GetSerializedHash(),
                Key = key,
                Network = authenticatedWallet.Network
            });
            var password = walletStore.GetPassword();
            _walletRepository.Update(authenticatedWallet, walletStore.GetPassword());
            return key;
        }
    }
}
