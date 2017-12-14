using System;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Events
{
    internal class FlyoutEventArgs : EventArgs
    {
        private readonly UserControl _flyout;

        public FlyoutEventArgs(UserControl flyout)
        {
            _flyout = flyout;
        }

        public UserControl Data
        {
            get
            {
                return _flyout;
            }
        }
    }
}
