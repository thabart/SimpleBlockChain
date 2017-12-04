using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Pages
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            var viewModel = new HomePageViewModel();
            DataContext = viewModel;
            viewModel.NavigateToWalletEvt += NavigateToWallet;
        }

        private void NavigateToWallet(object sender, EventArgs e)
        {
            NavigationService.Navigate(new AuthenticateWallet());
        }
    }
}
