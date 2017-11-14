using System.Collections.Generic;

namespace SimpleBlockChain.Core.Transactions
{
    public enum ScriptTypes
    {
        P2PKH,
        P2SH
    }

    public class Script
    {
        private readonly IEnumerable<byte> _payload;

        public Script(IEnumerable<byte> payload)
        {
            _payload = payload;
        }

        public IEnumerable<byte> Serialize()
        {
            return _payload;
        }
    }
}
