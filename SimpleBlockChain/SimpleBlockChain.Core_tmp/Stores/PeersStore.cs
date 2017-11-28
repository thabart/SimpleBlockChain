using SimpleBlockChain.Core.Messages.ControlMessages;

namespace SimpleBlockChain.Core.Stores
{
    public class PeersStore
    {
        private static PeersStore _instance;
        private IpAddress _myIpAddress;

        private PeersStore() { }

        public static PeersStore Instance()
        {
            if (_instance == null)
            {
                _instance = new PeersStore();
            }

            return _instance;
        }

        public void SetMyIpAddress(IpAddress ipAddress)
        {
            _myIpAddress = ipAddress;
        }

        public IpAddress GetMyIpAddress()
        {
            return _myIpAddress;
        }
    }
}