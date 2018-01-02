using MahApps.Metro.Controls.Dialogs;
using System;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly IDialogCoordinator _dialogCoordinator;

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
        }

        public void DisplayError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            _dialogCoordinator.ShowMessageAsync(this, "Error", message);
        }
    }
}
