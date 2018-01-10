using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Rpc.Parameters
{
    public class SmartContractTransactionParameter
    {
        public SmartContractTransactionParameter(IEnumerable<byte> to)
        {
            To = to;
            From = new byte[0];
            Data = new byte[0];
            Gas = 0;
            GasPrice = 0;
            Value = 0;
        }

        public IEnumerable<byte> To { get; set; }
        public IEnumerable<byte> From { get; set; }
        public IEnumerable<byte> Data { get; set; }
        public double Gas { get; set; }
        public double GasPrice { get; set; }
        public double Value { get; set; }
    }
}
