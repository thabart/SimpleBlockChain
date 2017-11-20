using System;

namespace SimpleBlockChain.Core.Messages.DataMessages
{
    public enum InventoryTypes : UInt32
    {
        MSG_TX = 1,
        MSG_BLOCK = 2,
        MSG_FILTERED_BLOCK = 3
    }
}
