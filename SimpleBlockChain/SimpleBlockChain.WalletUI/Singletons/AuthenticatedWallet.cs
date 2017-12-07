using SimpleBlockChain.Core.Aggregates;
using System;

namespace SimpleBlockChain.WalletUI.Singletons
{
    public class AuthenticatedWallet
    {
        private WalletAggregate _authenticatedWallet;
        private static AuthenticatedWallet _instance;

        public static AuthenticatedWallet Instance()
        {
            if (_instance != null)
            {
                _instance = new AuthenticatedWallet();
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
