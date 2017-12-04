using SimpleBlockChain.WalletUI.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Pages
{
    public partial class AuthenticateWallet : Page
    {
        private readonly AuthenticateWalletViewModel _viewModel;

        public AuthenticateWallet()
        {
            _viewModel = new AuthenticateWalletViewModel();
            InitializeComponent();
            DataContext = _viewModel;
            Loaded += Load;
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            var walletPath = GetWalletPath();
            if (!Directory.Exists(walletPath))
            {
                Directory.CreateDirectory(walletPath);
            }

            var wallets = Directory.GetFiles(walletPath, "*.json");
            foreach(var wallet in wallets)
            {
                var fileName = Path.GetFileName(wallet);
                _viewModel.Wallets.Add(new WalletItemViewModel
                {
                    Name = fileName,
                    Path = wallet
                });
            }
        }

        private static string GetWalletPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wallets");
        }
    }
}
