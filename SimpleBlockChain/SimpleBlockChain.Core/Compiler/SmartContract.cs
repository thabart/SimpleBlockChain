using System.Collections.Generic;

namespace SimpleBlockChain.Core.Compiler
{
    public class SmartContract
    {
        public IEnumerable<byte> Address { get; set; }
        public IEnumerable<byte> Code { get; set; }
    }
}
