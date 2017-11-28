using SimpleBlockChain.Core.Messages.ControlMessages;
using System;

namespace SimpleBlockChain.Core.Evts
{
    public class PeerEventStore
    {
        private static PeerEventStore _instance;

        public static PeerEventStore Instance()
        {
            if (_instance == null)
            {
                _instance = new PeerEventStore();
            }

            return _instance;
        }

        public void NewPeer(IpAddress ipAddress)
        {
            if (ipAddress == null)
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }

            if (NewPeerEvt != null)
            {
                NewPeerEvt(this, new IpAddressEventArgs(ipAddress));
            }
        }

        public event EventHandler<IpAddressEventArgs> NewPeerEvt;
    }
}
