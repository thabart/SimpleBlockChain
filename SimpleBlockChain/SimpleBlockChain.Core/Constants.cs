using System.Collections.Generic;
using System.Numerics;

namespace SimpleBlockChain.Core
{
    public static class Constants
    {
        public const int SupportedProtocolVersion = 70015;
        public const uint DEFAULT_NBITS = 0x1d00ffff;
        public const uint DEFAULT_GENESIS_NONCE = 2083236893;
        public const uint DEFAULT_GENESIS_SEQUENCE = 0xFFFFFFFF;
        public const uint DEFAULT_GENESIS_TRANSACTION_VERSION = 1;
        public const uint DEFAULT_GENESIS_TRANSACTION_LOCKTIME = 0;
        public static BigInteger DEFAULT_GENESIS_PUBLIC_KEY = BigInteger.Parse("60427808088723647717719642775002509788942878284828224005325482594598304990052926891905017266624126009707826164889663599685685485915832876111601670003239347");

        public class Ports
        {
            public const string MainNet = "8333";
            public const string TestNet = "18333";
            public const string RegTest = "18444";
        }

        public static List<string> MessageNameLst = new List<string> { Constants.MessageNames.Ping, MessageNames.Addr,
            MessageNames.Version, MessageNames.Verack, MessageNames.GetAddr, MessageNames.Inventory, MessageNames.Transaction };

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
        }
    }
}
