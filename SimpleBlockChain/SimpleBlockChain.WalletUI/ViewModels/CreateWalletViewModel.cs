using SimpleBlockChain.WalletUI.Commands;
using System.Security;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class CreateWalletViewModel : BaseViewModel
    {
        private readonly ICommand _createWallet;
        private SecureString _password;

        public CreateWalletViewModel()
        {
            _createWallet = new RelayCommand(p => CreateWalletExecute(), p => CanCreateWallet());
        }

        public ICommand CreateWallet
        {
            get
            {
                return _createWallet;
            }
        }

        public SecureString Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    NotifyPropertyChanged(nameof(Password));
                }
            }
        }

        public string WalletName { get; set; }

        private void CreateWalletExecute()
        {

        }

        private bool CanCreateWallet()
        {
            return true;
        }
    }
}