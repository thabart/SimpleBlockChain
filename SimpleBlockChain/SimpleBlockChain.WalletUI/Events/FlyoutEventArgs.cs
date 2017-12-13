using System;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Events
{
    internal class FlyoutEventArgs : EventArgs
    {
        private readonly Page _flyout;

        public FlyoutEventArgs(Page flyout)
        {
            _flyout = flyout;
        }

        public Page Data
        {
            get
            {
                return _flyout;
            }
        }
    }
}
