using MahApps.Metro.Controls;
using SimpleBlockChain.WalletUI.Pages;

namespace SimpleBlockChain.WalletUI
{
    public partial class MainWindow : MetroWindow
    {
        private HomePage _homePage;

        public MainWindow(HomePage homePage)
        {
            _homePage = homePage;
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.frame.Navigate(_homePage);
        }
    }
}
