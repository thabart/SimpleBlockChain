namespace SimpleBlockChain.Interop
{
    public enum RpcAuthentication : uint
    {
        RPC_C_AUTHN_NONE = 0,
        RPC_C_AUTHN_DCE_PRIVATE = 1,
        RPC_C_AUTHN_DCE_PUBLIC = 2,
        RPC_C_AUTHN_DEC_PUBLIC = 4,
        RPC_C_AUTHN_GSS_NEGOTIATE = 9,
        RPC_C_AUTHN_WINNT = 10,
        RPC_C_AUTHN_GSS_SCHANNEL = 14,
        RPC_C_AUTHN_GSS_KERBEROS = 16,
        RPC_C_AUTHN_DPA = 17,
        RPC_C_AUTHN_MSN = 18,
        RPC_C_AUTHN_DIGEST = 21,
        RPC_C_AUTHN_MQ = 100,
        RPC_C_AUTHN_DEFAULT = 4294967295,
    }
}
