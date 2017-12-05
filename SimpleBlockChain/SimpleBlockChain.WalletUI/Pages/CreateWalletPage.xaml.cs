using SimpleBlockChain.WalletUI.ViewModels;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Pages
{
    public partial class CreateWalletPage : Page
    {
        private readonly CreateWalletViewModel _viewModel;

        public CreateWalletPage()
        {
            InitializeComponent();
            _viewModel = new CreateWalletViewModel();
            DataContext = _viewModel;
        }
    }
}
