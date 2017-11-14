namespace SimpleBlockChain.Core.Transactions
{
    public class Outpoint
    {
        public char[] Hash { get; set; } // hash
        public uint Index { get; set; } // index
    }
}
