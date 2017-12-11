using System.Collections.Generic;
using System.Numerics;

namespace SimpleBlockChain.Core
{
    public static class Constants
    {
        // public const string DNS_IP_ADDRESS = "127.0.0.1";
        // public const string DNS_IP_ADDRESS = "169.254.84.193";
        public const string DNS_IP_ADDRESS = "192.168.76.130";
        public const int SupportedProtocolVersion = 70015;
        public const uint DEFAULT_NBITS = 0x1d00ffff;
        public const uint DEFAULT_GENESIS_NONCE = 2083236893;
        public const uint DEFAULT_GENESIS_SEQUENCE = 0xFFFFFFFF;
        public const uint DEFAULT_GENESIS_TRANSACTION_VERSION = 1;
        public const uint DEFAULT_GENESIS_TRANSACTION_LOCKTIME = 0;
        public const double DEFAULT_MIN_TX_FEE = 0.00001;
        public const int DEFAULT_NB_BLOCKS_PAST = 5;
        public const int DEFAULT_MAX_GET_INVENTORIES = 500;
        public const int MIN_NB_TRANSACTIONS_BLOCK = 1;
        public static BigInteger DEFAULT_GENESIS_PUBLIC_KEY_HASH = BigInteger.Parse("-714949846588763741693703373479372603078333912921");
        public const string DEFAULT_SIGNATURE_CONTENT = "simple_block_chain";

        public class Ports
        {
            public const string MainNet = "8333";
            public const string TestNet = "18333";
            public const string RegTest = "18444";
        }

        public class RpcPorts
        {
            public const string MainNet = "8332";
            public const string TestNet = "18332";
            public const string RegTest = "18332";
        }

        public static List<string> MessageNameLst = new List<string> { Constants.MessageNames.Ping, MessageNames.Addr,
            MessageNames.Version, MessageNames.Verack, MessageNames.GetAddr, MessageNames.Inventory,
            MessageNames.Transaction, MessageNames.Pong, MessageNames.MemPool,
            MessageNames.GetData, MessageNames.Block, MessageNames.GetBlocks, MessageNames.NotFound };

        public class MessageNames
        {
            public const string Ping = "ping";
            public const string Pong = "pong";
            public const string Addr = "addr";
            public const string Version = "version";
            public const string Verack = "verack";
            public const string GetAddr = "getaddr";
            public const string Transaction = "tx";
            public const string Inventory = "inv";
            public const string MemPool = "mempool";
            public const string GetData = "getdata";
            public const string Block = "block";
            public const string GetBlocks = "getblocks";
            public const string NotFound = "notfound";
        }

        public class RpcOperations
        {
            public const string Getblocktemplate = "getblocktemplate";
            public const string Submitblock = "submitblock";
            public const string Getrawmempool = "getrawmempool";
            public const string ListUnspent = "listunspent";
        }
    }
}
