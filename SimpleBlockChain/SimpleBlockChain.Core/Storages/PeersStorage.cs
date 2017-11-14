using Newtonsoft.Json;
using SimpleBlockChain.Core.Messages.ControlMessages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlockChain.Core.Storages
{
    public class PeersStorage
    {
        private const string _fileName = "peers.dat";

        public async Task<bool> AddPeer(IpAddress ipAddress)
        {
            if (ipAddress == null)
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }

            CheckFileExists();
            using (var file = new StreamWriter(_fileName))
            {
                var json = JsonConvert.SerializeObject(ipAddress);
                await file.WriteLineAsync(json).ConfigureAwait(false);
                return true;
            }
        }

        public IEnumerable<IpAddress> GetAll()
        {
            var lines = File.ReadAllLines(_fileName);
            return lines.Select(l => JsonConvert.DeserializeObject<IpAddress>(l));
        }

        private static void CheckFileExists()
        {
            if (!File.Exists(_fileName))
            {
                File.Create(_fileName);
            }
        }
    }
}
