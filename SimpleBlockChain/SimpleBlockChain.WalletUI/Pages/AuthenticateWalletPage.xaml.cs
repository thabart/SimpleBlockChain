using MahApps.Metro.Controls.Dialogs;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Pages
{
    public partial class AuthenticateWalletPage : Page
    {
        private readonly AuthenticateWalletViewModel _viewModel;
        private readonly IWalletRepository _walletRepository;
        private readonly WalletPage _walletPage;

        public AuthenticateWalletPage(IWalletRepository walletRepository, WalletPage walletPage)
        {
            _viewModel = new AuthenticateWalletViewModel(DialogCoordinator.Instance);
            _walletRepository = walletRepository;
            _walletPage = walletPage;
            _viewModel.ConnectEvt += Connect;
            InitializeComponent();
            DataContext = _viewModel;
            Loaded += Load;
        }

        private void Connect(object sender, EventArgs e)
        {
            if (_viewModel.SelectedWallet == null)
            {
                return;
            }

            _viewModel.ToggleLoading();
            _walletRepository.Get(_viewModel.SelectedWallet.Name, _viewModel.Password).ContinueWith((r) =>
            {
                try
                {
                    var result = r.Result;
                    WalletStore.Instance().SetAuthenticatedWallet(r.Result);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NavigationService.Navigate(_walletPage);
                    });
                }
                catch (AggregateException)
                {
                    _viewModel.DisplayMessage("Error", "Cannot connect to the wallet");
                }
                finally
                {
                    _viewModel.ToggleLoading();
                }
            });
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            _viewModel.Wallets.Clear();
            _walletRepository.GetAll().ContinueWith((r) =>
            {
                var names = r.Result;
                foreach (var name in names)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _viewModel.Wallets.Add(new WalletItemViewModel
                        {
                            Name = name
                        });
                    });
                }
            });
        }
    }
}
