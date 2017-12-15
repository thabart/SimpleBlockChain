using MahApps.Metro.Controls;
using System;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Events
{
    internal class FlyoutEventArgs : EventArgs
    {
        private readonly UserControl _flyout;
        private readonly Position _position;
        private readonly double _minWidth;

        public FlyoutEventArgs(UserControl flyout, Position position, double minWidth)
        {
            _flyout = flyout;
            _position = position;
            _minWidth = minWidth;
        }

        public UserControl UserControl
        {
            get
            {
                return _flyout;
            }
        }

        public Position Position
        {
            get
            {
                return _position;
            }
        }

        public double MinWidth
        {
            get
            {
                return _minWidth;
            }
        }
    }
}
