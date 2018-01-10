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
        public event EventHandler<ErrorEventArgs> DisplayErrorEvt;
        public event EventHandler<MessageEventArgs> DisplayMessageEvt;

        public void DisplayFlyout(UserControl flyout, Position position, double minWidth)
        {
            if (flyout == null)
            {
                throw new ArgumentNullException(nameof(flyout));
            }
            if (DisplayFlyoutEvt != null)
            {
                DisplayFlyoutEvt(this, new FlyoutEventArgs(flyout, position, minWidth));
            }
        }

        public void DisplayError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (DisplayErrorEvt != null)
            {
                DisplayErrorEvt(this, new ErrorEventArgs(message));
            }
        }

        public void DisplayMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (DisplayMessageEvt != null)
            {
                DisplayMessageEvt(this, new MessageEventArgs(message));
            }
        }
    }
}
