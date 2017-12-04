using SimpleBlockChain.WalletUI.Commands;
using System;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class HomePageViewModel
    {
        private readonly ICommand _connectWalletCommand;

        public HomePageViewModel()
        {
            _connectWalletCommand = new RelayCommand(p => ConnectWalletExecute(), p => CanConnectWallet());
        }

        public event EventHandler NavigateToWalletEvt;

        public ICommand ConnectWallet
        {
            get
            {
                return _connectWalletCommand;
            }
        }

        private bool CanConnectWallet()
        {
            return true;
        }

        private void ConnectWalletExecute()
        {
            if (NavigateToWalletEvt != null)
            {
                NavigateToWalletEvt(this, EventArgs.Empty);
            }
        }
    }
}
