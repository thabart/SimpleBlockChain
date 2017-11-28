namespace SimpleBlockChain.Core.States
{
    public class PeerConnection
    {
        public PeerConnection(byte[] ipAddress)
        {
            IpAddress = ipAddress;
            State = PeerConnectionStates.NotConnected;
        }

        public void Accept()
        {
            State = PeerConnectionStates.Accepted;
        }

        public void Connect()
        {
            State = PeerConnectionStates.Connected;
        }

        public void Disconnect()
        {
            State = PeerConnectionStates.NotConnected;
        }

        public byte[] IpAddress { get; private set;}
        public PeerConnectionStates State { get; private set; }
    }
}
