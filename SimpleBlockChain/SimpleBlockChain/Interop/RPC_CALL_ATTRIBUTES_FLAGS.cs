using System;

namespace SimpleBlockChain.Interop
{
    [Flags]
    internal enum RPC_CALL_ATTRIBUTES_FLAGS
    {
        RPC_QUERY_SERVER_PRINCIPAL_NAME = 2,
        RPC_QUERY_CLIENT_PRINCIPAL_NAME = 4,
        RPC_QUERY_CALL_LOCAL_ADDRESS = 8,
        RPC_QUERY_CLIENT_PID = 16,
        RPC_QUERY_IS_CLIENT_LOCAL = 32,
        RPC_QUERY_NO_AUTH_REQUIRED = 64,
    }
}
