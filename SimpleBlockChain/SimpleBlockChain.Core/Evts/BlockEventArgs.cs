using SimpleBlockChain.Core.Blocks;
using System;

namespace SimpleBlockChain.Core.Evts
{
    public class BlockEventArgs : EventArgs
    {
        public Block Data { get; set; }

        public BlockEventArgs(Block data)
        {
            Data = data;
        }
    }
}
