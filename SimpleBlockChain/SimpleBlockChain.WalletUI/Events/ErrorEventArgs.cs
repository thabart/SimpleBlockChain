using System;

namespace SimpleBlockChain.WalletUI.Events
{
    public class ErrorEventArgs : EventArgs
    {
        private readonly string _error;

        public ErrorEventArgs(string error)
        {
            _error = error;
        }

        public string Data
        {
            get
            {
                return _error;
            }
        }
    }
}
