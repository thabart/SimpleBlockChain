using MahApps.Metro.Controls;
using SimpleBlockChain.WalletUI.Pages;

namespace SimpleBlockChain.WalletUI
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.frame.Navigate(new HomePage());
        }
    }
}
