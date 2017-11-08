using System;

namespace SimpleBlockChain.Core
{
    public enum ServiceFlags : UInt64
    {
        // Nothing
        NODE_NONE = 0,
        // NODE_NETWORK means that the node is capable of serving the block chain. It is currently
        // set by all Bitcoin Core nodes, and is unset by SPV clients or other peers that just want
        // network services but don't provide them.
        NODE_NETWORK = (1 << 0),
        // NODE_GETUTXO means the node is capable of responding to the getutxo protocol request.
        // Bitcoin Core does not support this but a patch set called Bitcoin XT does.
        // See BIP 64 for details on how this is implemented.
        NODE_GETUTXO = (1 << 1),
        // NODE_BLOOM means the node is capable and willing to handle bloom-filtered connections.
        // Bitcoin Core nodes used to support this by default, without advertising this bit,
        // but no longer do as of protocol version 70011 (= NO_BLOOM_VERSION)
        NODE_BLOOM = (1 << 2),
        // NODE_WITNESS indicates that a node can be asked for blocks and transactions including
        // witness data.
        NODE_WITNESS = (1 << 3),
        // NODE_XTHIN means the node supports Xtreme Thinblocks
        // If this is turned off then the node will not service nor make xthin requests
        NODE_XTHIN = (1 << 4)
    }
}
