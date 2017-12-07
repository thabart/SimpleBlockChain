using SimpleBlockChain.WalletUI.Commands;
using System.Security;
using System.Windows.Input;
using System;
using MahApps.Metro.Controls.Dialogs;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class CreateWalletViewModel : BaseViewModel
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly ICommand _createWallet;
        private SecureString _password;
        private bool _isNotLoading = false;

        public CreateWalletViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
            _isNotLoading = true;
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

        public void ToggleLoading()
        {
            IsNotLoading = !_isNotLoading;
        }

        public void DisplayMessage(string title, string message)
        {
            _dialogCoordinator.ShowMessageAsync(this, title, message);
        }

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