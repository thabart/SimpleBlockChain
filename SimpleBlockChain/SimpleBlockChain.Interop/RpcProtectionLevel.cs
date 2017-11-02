namespace SimpleBlockChain.Interop
{
    public enum RpcProtectionLevel : uint
    {
        RPC_C_PROTECT_LEVEL_DEFAULT,
        RPC_C_PROTECT_LEVEL_NONE,
        RPC_C_PROTECT_LEVEL_CONNECT,
        RPC_C_PROTECT_LEVEL_CALL,
        RPC_C_PROTECT_LEVEL_PKT,
        RPC_C_PROTECT_LEVEL_PKT_INTEGRITY,
        RPC_C_PROTECT_LEVEL_PKT_PRIVACY,
    }
}
