using MahApps.Metro.Controls.Dialogs;
using SimpleBlockChain.Core.Aggregates;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Pages
{
    public partial class CreateWalletPage : Page
    {
        private readonly IWalletRepository _walletRepository;
        private readonly CreateWalletViewModel _viewModel;

        public CreateWalletPage(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
            _viewModel = new CreateWalletViewModel(DialogCoordinator.Instance);
            _viewModel.CreateWalletEvt += CreateWallet;
            InitializeComponent();
            DataContext = _viewModel;
        }

        private void CreateWallet(object sender, EventArgs e)
        {
            if (_viewModel.Password == null || string.IsNullOrWhiteSpace(_viewModel.WalletName))
            {
                return;
            }

            var record = new WalletAggregate
            {
                Name = _viewModel.WalletName
            };

            _viewModel.ToggleLoading();
            _walletRepository.Add(record, _viewModel.Password).ContinueWith((r) =>
            {
                try
                {
                    var b = r.Result;
                }
                catch
                {
                    _viewModel.DisplayMessage("Error", "An error occured while trying to create the wallet");
                }
                finally
                {
                    _viewModel.ToggleLoading();
                }
            });
        }
    }
}
