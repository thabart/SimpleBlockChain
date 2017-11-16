namespace SimpleBlockChain.Core.Transactions
{
    public enum OpCodes : byte
    {
        OP_DATA = 0x00, // Push data to a stack.
        OP_DUP = 0x76,
        OP_HASH160 = 0xa9,
        OP_EQUALVERIFY = 0x88,
        OP_CHECKSIG = 0xac
    }
}
