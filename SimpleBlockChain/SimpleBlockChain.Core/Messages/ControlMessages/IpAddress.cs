using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Converters;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Messages.ControlMessages
{
    public class IpAddress
    {
        public const int Size = 34;
        public DateTime Time { get; private set; }
        public ServiceFlags ServiceFlag { get; private set; }
        [JsonConverter(typeof(ByteArrayConverter))]
        public byte[] Ipv6 { get; private set; }
        public UInt16 Port { get; private set; }

        public IpAddress()
        {

        }

        public IpAddress(ServiceFlags serviceFlag, byte[] ipv6, UInt16 port)
        {
            ServiceFlag = serviceFlag;
            Ipv6 = ipv6;
            Port = port;
        }

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

            var unixTimePayload = payload.Take(8).ToArray();
            var time = BitConverter.ToInt64(unixTimePayload, 0).ToDateTime();
            var serviceFlags = (ServiceFlags)(BitConverter.ToUInt64(payload.Skip(8).Take(8).ToArray(), 0));
            var ipv6 = payload.Skip(16).Take(16).ToArray();
            var portPayload = payload.Skip(32).Take(2);
            var port = BitConverter.ToUInt16(portPayload.ToArray(), 0);
            return new IpAddress(time, serviceFlags, ipv6, port);
        }

        public static IpAddress Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentNullException(nameof(json));
            }

            var jToken = JToken.Parse(json);
            var time = jToken.Value<DateTime>("Time");
            var serviceFlag = (ServiceFlags)jToken.Value<UInt64>("ServiceFlag");
            var ipV6Token = jToken.SelectToken("Ipv6") as JArray;
            var ipV6 = new List<byte>();
            foreach(var i  in ipV6Token)
            {
                ipV6.Add((byte)i);
            }

            var port = jToken.Value<UInt16>("Port");
            return new IpAddress(time, serviceFlag, ipV6.ToArray(), port);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var ipAddr = obj as IpAddress;
            if (ipAddr == null)
            {
                return false;
            }

            return Ipv6.SequenceEqual(ipAddr.Ipv6);
        }
    }
}
