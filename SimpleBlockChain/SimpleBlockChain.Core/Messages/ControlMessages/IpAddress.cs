using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Messages.ControlMessages
{
    public class IpAddress
    {
        public const int Size = 30;
        public DateTime Time { get; private set; }
        public ServiceFlags ServiceFlag { get; private set; }
        public byte[] Ipv6 { get; private set; }
        public UInt16 Port { get; private set; }

        public IpAddress(DateTime time, ServiceFlags serviceFlag, byte[] ipv6, UInt16 port)
        {
            Time = time;
            ServiceFlag = serviceFlag;
            Ipv6 = ipv6;
            Port = port;
        }
        
        public byte[] Serialize()
        {
            var result = new List<byte>();
            var unixTime = Time.ToUnixTime();
            result.AddRange(BitConverter.GetBytes(unixTime));
            result.AddRange(BitConverter.GetBytes((UInt64)ServiceFlag));
            result.AddRange(Ipv6);
            result.AddRange(BitConverter.GetBytes(Port));
            return result.ToArray();
        }

        public static IpAddress Deserialize(byte[] payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (payload.Length != Size)
            {
                throw new SerializeException(ErrorCodes.PayloadSizeInvalid);
            }

            var unixTimePayload = payload.Take(4).ToArray();
            var time = BitConverter.ToUInt32(unixTimePayload, 0).ToDateTime();
            var serviceFlags = (ServiceFlags)(BitConverter.ToUInt64(payload.Skip(4).Take(8).ToArray(), 0));
            var ipv6 = payload.Skip(12).Take(16).ToArray();
            var portPayload = payload.Skip(28).Take(2);
            var port = BitConverter.ToUInt16(portPayload.ToArray(), 0);
            return new IpAddress(time, serviceFlags, ipv6, port);
        }
    }
}
