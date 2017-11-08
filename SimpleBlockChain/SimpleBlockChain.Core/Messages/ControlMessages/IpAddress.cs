using System;

namespace SimpleBlockChain.Core.Messages.ControlMessages
{
    public class IpAddress
    {
        public DateTime Time { get; private set; }
        public ServiceFlags ServiceFlag { get; private set; }
        public string Ip { get; private set; }
        public UInt16 Port { get; private set; }

        public IpAddress(DateTime time, ServiceFlags serviceFlag, string ip, UInt16 port)
        {
            Time = time;
            ServiceFlag = serviceFlag;
            Ip = ip;
            Port = port;
        }
        
        public byte[] Serialize()
        {
            return null;
        }
    }
}
