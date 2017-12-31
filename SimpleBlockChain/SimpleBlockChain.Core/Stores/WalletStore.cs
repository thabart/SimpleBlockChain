using SimpleBlockChain.Core.Aggregates;
using System;
using System.Security;

namespace SimpleBlockChain.Core.Stores
{
    public class WalletStore
    {
        private WalletAggregate _authenticatedWallet;
        private SecureString _password;
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

        public void SetPassword(SecureString password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            _password = password;
        }

        public void Switch(Networks network)
        {
            if (_authenticatedWallet == null)
            {
                return;
            }

            _authenticatedWallet.Network = network;
        }

        public WalletAggregate GetAuthenticatedWallet()
        {
            return _authenticatedWallet;
        }

        public SecureString GetPassword()
        {
            return _password;
        }
    }
}
