using Newtonsoft.Json;
using SimpleBlockChain.Core.Messages.ControlMessages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleBlockChain.Core.Storages
{
    public class PeersRepository
    {
        private const string _fileName = "peers.dat";
        private object obj = new object();

        public bool AddPeers(IEnumerable<IpAddress> ipAddresses)
        {
            if (ipAddresses == null)
            {
                throw new ArgumentNullException(nameof(ipAddresses));
            }

            foreach(var ipAddress in ipAddresses)
            {
                AddPeer(ipAddress);
            }

            return true;
        }

        public bool AddPeer(IpAddress ipAddress)
        {
            if (ipAddress == null)
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }

            CheckFileExists();
            var ipAddressLst = GetAll();
            if (ipAddressLst.Any(ipa => ipa.Ipv6.SequenceEqual(ipAddress.Ipv6)))
            {
                return false;
            }
            lock (obj)
            {
                using (var file = new StreamWriter(File.Open(GetPath(), FileMode.Open)))
                {
                    var json = JsonConvert.SerializeObject(ipAddress);
                    file.WriteLine(json);
                }
            }

            return true;
        }

        public bool RemovePeer(IpAddress ipAddress)
        {
            if (ipAddress == null)
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }

            CheckFileExists();
            var ipAddressLst = GetAll().Where(ip => !ip.Ipv6.SequenceEqual(ipAddress.Ipv6));
            lock (obj)
            {
                File.WriteAllText(GetPath(), string.Empty);
                using (var file = new StreamWriter(File.Open(GetPath(), FileMode.Open)))
                {
                    foreach (var adr in ipAddressLst)
                    {
                        var json = JsonConvert.SerializeObject(adr);
                        file.WriteLine(json);
                    }
                }

                return true;
            }
        }

        public IEnumerable<IpAddress> GetAll()
        {
            if (!File.Exists(GetPath()))
            {
                return new List<IpAddress>();
            }

            lock (obj)
            {
                var lines = File.ReadAllLines(GetPath());
                return lines.Select(l => IpAddress.Deserialize(l));
            }
        }

        public bool Empty()
        {
            if (!File.Exists(GetPath()))
            {
                return false;
            }

            lock(obj)
            {
                File.WriteAllText(GetPath(), string.Empty);
            }

            return true;
        }

        private static void CheckFileExists()
        {
            if (!File.Exists(GetPath()))
            {
                File.Create(GetPath()).Dispose();
            }
        }

        private static string GetPath()
        {
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return Path.Combine(path, _fileName);
        }
    }
}
