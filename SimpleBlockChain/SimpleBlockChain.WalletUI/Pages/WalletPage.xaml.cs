using SimpleBlockChain.WalletUI.ViewModels;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Pages
{
    public partial class WalletPage : Page
    {
        private readonly WalletPageViewModel _viewModel;

        public WalletPage(WalletPageViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.SendMoneyEvt += _viewModel_SendMoneyEvt;
            InitializeComponent();
        }

        private void _viewModel_SendMoneyEvt(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
