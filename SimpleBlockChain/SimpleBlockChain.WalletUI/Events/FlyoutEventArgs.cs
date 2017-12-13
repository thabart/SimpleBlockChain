using MahApps.Metro.Controls;
using System;

namespace SimpleBlockChain.WalletUI.Events
{
    internal class FlyoutEventArgs : EventArgs
    {
        private readonly Flyout _flyout;

        public FlyoutEventArgs(Flyout flyout)
        {
            _flyout = flyout;
        }

        public Flyout Data
        {
            get
            {
                return _flyout;
            }
        }
    }
}
