using SimpleBlockChain.Core.Repositories;
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

        public AuthenticateWalletPage(AuthenticateWalletViewModel viewModel, IWalletRepository walletRepository)
        {
            _viewModel = viewModel;
            _viewModel.ConnectEvt += Connect;
            _walletRepository = walletRepository;
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

            _walletRepository.Get(_viewModel.SelectedWallet.Name, _viewModel.Password).ContinueWith((r) =>
            {
                try
                {
                    var result = r.Result;
                    string s = "";
                }
                catch (AggregateException ex)
                {

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
