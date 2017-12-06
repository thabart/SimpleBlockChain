using SimpleBlockChain.WalletUI.ViewModels;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Pages
{
    public partial class CreateWalletPage : Page
    {
        private readonly CreateWalletViewModel _viewModel;

        public CreateWalletPage(CreateWalletViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
