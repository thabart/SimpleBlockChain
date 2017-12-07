using MahApps.Metro.Controls.Dialogs;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.WalletUI.Singletons;
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
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly WalletPage _walletPage;

        public AuthenticateWalletPage(AuthenticateWalletViewModel viewModel, 
            IWalletRepository walletRepository, IDialogCoordinator dialogCoordinator, WalletPage walletPage)
        {
            _viewModel = viewModel;
            _walletRepository = walletRepository;
            _dialogCoordinator = dialogCoordinator;
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
                    AuthenticatedWallet.Instance().SetAuthenticatedWallet(r.Result);
                    NavigationService.Navigate(_walletPage);
                }
                catch (AggregateException)
                {
                    _dialogCoordinator.ShowMessageAsync(this, "Error", "Cannot connect to the wallet");
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
