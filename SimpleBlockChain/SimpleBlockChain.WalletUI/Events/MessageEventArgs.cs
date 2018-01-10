using System;

namespace SimpleBlockChain.WalletUI.Events
{
    public class MessageEventArgs : EventArgs
    {
        private readonly string _message;

        public MessageEventArgs(string message)
        {
            _message = message;
        }

        public string Data
        {
            get
            {
                return _message;
            }
        }
    }
}
