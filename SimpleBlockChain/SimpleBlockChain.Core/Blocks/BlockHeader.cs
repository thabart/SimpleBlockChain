namespace SimpleBlockChain.Core.Blocks
{
    public class BlockHeader
    {
        public int Version { get; set; }
        public char[] PreviousBlockHeader { get; set; }
        public char[] MerkleRoot { get; set; }
        public int Time { get; set; }
        public uint NBits { get; set; }
        public uint Nonce { get; set; }
    }
}
