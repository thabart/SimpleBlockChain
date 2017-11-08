using System.Collections.Generic;

namespace SimpleBlockChain.Core
{
    public static class Constants
    {
        public class Ports
        {
            public const string MainNet = "8333";
            public const string TestNet = "18333";
            public const string RegTest = "18444";
        }

        public static List<string> MessageNameLst = new List<string> { Constants.MessageNames.Ping };

        public class MessageNames
        {
            public const string Ping = "ping";
            public const string Pong = "pong";
        }
    }
}
