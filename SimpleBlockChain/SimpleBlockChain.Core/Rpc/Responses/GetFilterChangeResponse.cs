using SimpleBlockChain.Core.Compiler;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Rpc.Responses
{
    public class GetFilterChangeResponse
    {
        public IEnumerable<byte> BlockHash { get; set; }
        public IEnumerable<byte> BlockNumber { get; set; }
        public IEnumerable<byte> Address { get; set; }
        public IEnumerable<byte> Data { get; set; }
        public IEnumerable<DataWord> Topics { get; set; }
    }
}
