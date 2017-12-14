using MahApps.Metro.Controls;
using SimpleBlockChain.WalletUI.Events;
using System;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Stores
{
    internal class MainWindowStore
    {
        private static MainWindowStore _instance;

        public static MainWindowStore Instance()
        {
            if (_instance == null)
            {
                _instance = new MainWindowStore();
            }

            return _instance;
        }

        public event EventHandler<FlyoutEventArgs> DisplayFlyoutEvt;

        public void DisplayFlyout(UserControl flyout)
        {
            if (flyout == null)
            {
                throw new ArgumentNullException(nameof(flyout));
            }
            if (DisplayFlyoutEvt != null)
            {
                DisplayFlyoutEvt(this, new FlyoutEventArgs(flyout));
            }
        }
    }
}
