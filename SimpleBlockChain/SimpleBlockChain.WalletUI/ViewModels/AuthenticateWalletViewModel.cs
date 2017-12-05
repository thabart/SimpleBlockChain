using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.WalletUI.Commands;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class WalletItemViewModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public class AuthenticateWalletViewModel : BaseViewModel
    {
        private readonly ICommand _authenticateWalletCommand;
        private SecureString _password;

        public AuthenticateWalletViewModel()
        {
            _authenticateWalletCommand = new RelayCommand(p => AuthenticateWalletCommand(), p => CanAuthenticateWalletCommand());
            Wallets = new ObservableCollection<WalletItemViewModel>();
        }

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

        public WalletItemViewModel SelectedWallet { get; set; }

        private void AuthenticateWalletCommand()
        {
            if (SelectedWallet == null || !File.Exists(SelectedWallet.Path))
            {
                return;
            }
            var keyRepository = new KeyRepository(SelectedWallet.Path);
            keyRepository.Load(_password.ToString());
            string s = "";
        }

        private bool CanAuthenticateWalletCommand()
        {
            return true;
        }
    }
}
