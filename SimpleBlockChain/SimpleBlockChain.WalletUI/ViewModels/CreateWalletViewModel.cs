using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.WalletUI.Commands;
using System.Security;
using System.Windows.Input;
using SimpleBlockChain.Core.Aggregates;
using System;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class CreateWalletViewModel : BaseViewModel
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ICommand _createWallet;
        private SecureString _password;

        public CreateWalletViewModel(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
            _createWallet = new RelayCommand(p => CreateWalletExecute(), p => CanCreateWallet());
        }

        public event EventHandler CreateWalletEvt;

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
            if (CreateWalletEvt != null)
            {
                CreateWalletEvt(this, EventArgs.Empty);
            }
        }

        private bool CanCreateWallet()
        {
            return true;
        }
    }
}