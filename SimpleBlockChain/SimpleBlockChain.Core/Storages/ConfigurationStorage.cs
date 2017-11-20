using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.States;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Storages
{
    public class ConfigurationStorage
    {
        private static ConfigurationStorage _instance;
        private IpAddress _myIpAddress;
        private IList<PeerConnection> _peerConnectionLst;

        public ConfigurationStorage()
        {
            _peerConnectionLst = new List<PeerConnection>();
        }

        public static ConfigurationStorage Instance()
        {
            if (_instance == null)
            {
                _instance = new ConfigurationStorage();
            }

            return _instance;
        }

        public IList<PeerConnection> GetPeerConnectionLst()
        {
            return _peerConnectionLst;
        }

        public void AddPeerConnection(PeerConnection peerConnection)
        {
            if (peerConnection == null)
            {
                throw new ArgumentNullException(nameof(peerConnection));
            }

            var pc = _peerConnectionLst.FirstOrDefault(p => p.IpAddress.SequenceEqual(peerConnection.IpAddress));
            if (pc != null) { return; }
            _peerConnectionLst.Add(peerConnection);
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
