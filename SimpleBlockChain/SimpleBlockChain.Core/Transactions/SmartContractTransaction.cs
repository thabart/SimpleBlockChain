using System.Collections.Generic;

namespace SimpleBlockChain.Core.Transactions
{
    public class SmartContractTransaction : BaseTransaction
    {
        public Script Script { get; set; }
        public string Author { get; set; }
        public string Name { get; set; }
        public IList<string> Parameters { get; set; }
        public long Gas { get; set; }

        public override int CompareTo(BaseTransaction obj)
        {
            return 0;
        }

        public override KeyValuePair<List<BaseTransactionIn>, int> DeserializeInputs(IEnumerable<byte> payload, int size)
        {
            return new KeyValuePair<List<BaseTransactionIn>, int>(null, 0);
        }
    }
}
