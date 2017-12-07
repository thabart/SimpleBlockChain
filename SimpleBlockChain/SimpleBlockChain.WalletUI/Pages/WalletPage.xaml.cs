using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Pages
{
    public partial class WalletPage : Page
    {
        private readonly WalletPageViewModel _viewModel;

        public WalletPage(WalletPageViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.SendMoneyEvt += SendMoney;
            InitializeComponent();
        }

        private void SendMoney(object sender, EventArgs e)
        {

        }
    }
}
