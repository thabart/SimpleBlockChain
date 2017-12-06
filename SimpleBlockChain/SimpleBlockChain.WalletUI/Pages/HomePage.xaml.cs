using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Pages
{
    public partial class HomePage : Page
    {
        private CreateWalletPage _createWalletPage;
        private AuthenticateWalletPage _authenticateWalletPage;

        public HomePage(HomePageViewModel homePageViewModel, CreateWalletPage createWalletPage, AuthenticateWalletPage authenticateWalletPage)
        {
            _createWalletPage = createWalletPage;
            _authenticateWalletPage = authenticateWalletPage;
            InitializeComponent();
            DataContext = homePageViewModel;
            homePageViewModel.NavigateToWalletEvt += NavigateToWallet;
            homePageViewModel.CreateWalletEvt += NavigateToCreateWallet;
        }

        private void NavigateToWallet(object sender, EventArgs e)
        {
            NavigationService.Navigate(_authenticateWalletPage);
        }

        private void NavigateToCreateWallet(object sender, EventArgs e)
        {
            NavigationService.Navigate(_createWalletPage);
        }
    }
}
