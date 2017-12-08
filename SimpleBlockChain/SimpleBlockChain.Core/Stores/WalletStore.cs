using SimpleBlockChain.Core.Aggregates;
using System;

namespace SimpleBlockChain.Core.Stores
{
    public class WalletStore
    {
        private WalletAggregate _authenticatedWallet;
        private static WalletStore _instance;

        public static WalletStore Instance()
        {
            if (_instance == null)
            {
                _instance = new WalletStore();
            }

            return _instance;
        }

        public void SetAuthenticatedWallet(WalletAggregate wallet)
        {
            if (wallet == null)
            {
                throw new ArgumentNullException(nameof(wallet));
            }

            _authenticatedWallet = wallet;
        }

        public WalletAggregate GetAuthenticatedWallet()
        {
            return _authenticatedWallet;
        }
    }
}
