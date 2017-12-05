using SimpleBlockChain.WalletUI.Commands;
using System;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class HomePageViewModel
    {
        private readonly ICommand _connectWalletCommand;
        private readonly ICommand _createWalletCommand;

        public HomePageViewModel()
        {
            _connectWalletCommand = new RelayCommand(p => ConnectWalletExecute(), p => CanConnectWallet());
            _createWalletCommand = new RelayCommand(p => CreateWalletExecute(), p => CanCreateWallet());
        }

        public event EventHandler NavigateToWalletEvt;
        public event EventHandler CreateWalletEvt;

        public ICommand ConnectWallet
        {
            get
            {
                return _connectWalletCommand;
            }
        }

        public ICommand CreateWallet
        {
            get
            {
                return _createWalletCommand;
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

        private bool CanCreateWallet()
        {
            return true;
        }

        private void CreateWalletExecute()
        {
            if (CreateWalletEvt != null)
            {
                CreateWalletEvt(this, EventArgs.Empty);
            }
        }
    }
}
