using System;

namespace SimpleBlockChain.Core.Evts
{
    public class StringEventArgs : EventArgs
    {
        public string Data { get; set; }
        public StringEventArgs(string data)
        {
            Data = data;
        }
    }
}
