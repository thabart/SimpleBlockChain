using System.Collections.Generic;

namespace SimpleBlockChain.Core.Blocks
{
    public class BlockChain
    {
        public BlockChain()
        {
            Blocks = new List<Block>
            {
                Block.GetGenesisBlock()
            };
        }

        public IEnumerable<Block> Blocks { get; private set; }
    }
}
