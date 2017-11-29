using SimpleBlockChain.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace SimpleBlockChain.Core.Helpers
{
    public class IpAdrHelper
    {
        public byte[] GetIpv6Address()
        {
            var hostName = Dns.GetHostName();            
            var ipEntry = Dns.GetHostEntryAsync(hostName).Result;
            var addr = ipEntry.AddressList;
            var ipv6Addr = addr.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetworkV6);
            if (ipv6Addr == null)
            {
                return null;
            }

            return ipv6Addr.GetAddressBytes();
        }

        public byte[] GetIpv4Address()
        {
            var hostName = Dns.GetHostName();
            var ipEntry = Dns.GetHostEntryAsync(hostName).Result;
            var addr = ipEntry.AddressList;
            var ipv4Addr = addr.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            if (ipv4Addr == null)
            {
                return null;
            }

            return ipv4Addr.MapToIPv6().GetAddressBytes();
        }

        public byte[] ParseRpcIpAddress(IRpcClientInfo client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var ip = new List<byte>();
            for(var i = 0; i < 10; i++)
            {
                ip.Add(0x00);
            }

            ip.Add(255);
            ip.Add(255);
            ip.AddRange(client.ClientAddress.Skip(4).Take(4));
            return ip.ToArray();
        }
    }
}
