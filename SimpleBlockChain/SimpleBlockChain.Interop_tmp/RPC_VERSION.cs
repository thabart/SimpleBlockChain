namespace SimpleBlockChain.Interop
{
    internal struct RPC_VERSION
    {
        public static readonly RPC_VERSION INTERFACE_VERSION = new RPC_VERSION()
        {
            MajorVersion = 1,
            MinorVersion = 0
        };
        public static readonly RPC_VERSION SYNTAX_VERSION = new RPC_VERSION()
        {
            MajorVersion = 2,
            MinorVersion = 0
        };
        public ushort MajorVersion;
        public ushort MinorVersion;
    }
}
