using SimpleBlockChain.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class SmartContractTransaction  : BaseTransaction // https://github.com/ethereum/wiki/wiki/JSON-RPC#eth_sendtransaction
    {
        public IEnumerable<byte> From { get; set; }
        public IEnumerable<byte> To { get; set; }
        public IEnumerable<byte> Data { get; set; }
        public double Gas { get; set; }
        public double GasPrice { get; set; }
        public double Value { get; set; }
        public int Nonce { get; set; }

        public override IEnumerable<byte> Serialize()
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(Version));
            var fromCompactSize = new CompactSize();
            var toCompactSize = new CompactSize();
            var dataCompactSize = new CompactSize();
            fromCompactSize.Size = (ulong)From.Count();
            toCompactSize.Size = To == null ? 0 : (ulong)To.Count();
            dataCompactSize.Size = Data == null ? 0 : (ulong)Data.Count();
            result.AddRange(fromCompactSize.Serialize());
            result.AddRange(From);
            result.AddRange(toCompactSize.Serialize());
            result.AddRange(To);
            result.AddRange(dataCompactSize.Serialize());
            result.AddRange(Data);
            result.AddRange(BitConverter.GetBytes(Gas));
            result.AddRange(BitConverter.GetBytes(GasPrice));
            result.AddRange(BitConverter.GetBytes(Value));
            result.AddRange(BitConverter.GetBytes(Nonce));
            result.AddRange(BitConverter.GetBytes(LockTime));
            return null;
        }
    }
}
