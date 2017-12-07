using SimpleBlockChain.WalletUI.Commands;
using System;
using System.Collections.ObjectModel;
using System.Security;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class WalletItemViewModel
    {
        public string Name { get; set; }
    }

    public class AuthenticateWalletViewModel : BaseViewModel
    {
        private readonly ICommand _authenticateWalletCommand;
        private SecureString _password;
        private bool _isNotLoading = false;

        public AuthenticateWalletViewModel()
        {
            Wallets = new ObservableCollection<WalletItemViewModel>();
            _authenticateWalletCommand = new RelayCommand(p => AuthenticateWalletCommand(), p => CanAuthenticateWalletCommand());
        }

        public event EventHandler ConnectEvt;

        public ObservableCollection<WalletItemViewModel> Wallets { get; private set; }

        public ICommand AuthenticateWallet
        {
            get
            {
                return _authenticateWalletCommand;
            }
        }

        public SecureString Password
        {
            get { return _password; }
            set
            {
                if (value != _password)
                {
                    _password = value;
                    NotifyPropertyChanged(nameof(Password));
                }
            }
        }

        public bool IsNotLoading
        {
            get
            {
                return _isNotLoading;
            }
            private set
            {
                if (value != _isNotLoading)
                {
                    _isNotLoading = value;
                    NotifyPropertyChanged(nameof(IsNotLoading));
                }
            }
        }

        public WalletItemViewModel SelectedWallet { get; set; }

        public void ToggleLoading()
        {
            IsNotLoading = !_isNotLoading;
        }

        private void AuthenticateWalletCommand()
        {
            if (ConnectEvt != null)
            {
                ConnectEvt(this, EventArgs.Empty);
            }
        }

        private bool CanAuthenticateWalletCommand()
        {
            return true;
        }
    }
}
