using System;

namespace SimpleBlockChain.WalletUI.Events
{
    public class BlockEventArgs : EventArgs
    {
        private readonly string _hash;

        public BlockEventArgs(string hash)
        {
            _hash = hash;
        }

        public string Data
        {
            get
            {
                return _hash;
            }
        }
    }
}
