using System.Collections.Generic;

namespace SimpleBlockChain.Core.Transactions
{
    public abstract class BaseTransactionOut
    {
        public Script Script { get; set; }

        public BaseTransactionOut(Script script)
        {
            Script = script;
        }

        public abstract IEnumerable<byte> Serialize();
    }
}
