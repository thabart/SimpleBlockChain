namespace SimpleBlockChain.Core.Transactions
{
    public enum OpCodes : byte
    {
        OP_DATA = 0x00, // Push data to a stack.
        OP_1 = 0x51, // STACK
        OP_2 = 0x52,
        OP_3 = 0x53,
        OP_4 = 0x54,
        OP_5 = 0x55,
        OP_6 = 0x56,
        OP_7 = 0x57,
        OP_8 = 0x58,
        OP_9 = 0x59,
        OP_10 = 0x60,
        OP_11 = 0x61,
        OP_12 = 0x62,
        OP_13 = 0x63,
        OP_14 = 0x64,
        OP_15 = 0x65,
        OP_16 = 0x66,
        OP_VERIFY = 0x69, // FLOW CONTROL.
        OP_EQUAL = 0x87, // BITWISE LOGIC.
        OP_ADD = 0x93, // ARITHMETIC
        OP_DUP = 0x76,
        OP_HASH160 = 0xa9,
        OP_EQUALVERIFY = 0x88,
        OP_CHECKSIG = 0xac,
    }
}
