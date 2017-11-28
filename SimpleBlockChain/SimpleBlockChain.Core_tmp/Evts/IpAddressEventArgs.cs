using SimpleBlockChain.Core.Messages.ControlMessages;
using System;

namespace SimpleBlockChain.Core.Evts
{
    public class IpAddressEventArgs : EventArgs
    {
        public IpAddress Data { get; set; }
        public IpAddressEventArgs(IpAddress data)
        {
            Data = data;
        }
    }
}
