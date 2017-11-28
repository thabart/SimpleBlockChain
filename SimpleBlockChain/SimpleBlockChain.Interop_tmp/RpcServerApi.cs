using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace SimpleBlockChain.Interop
{
    public class RpcServerApi : IDisposable
    {
        private static readonly UsageCounter _listenerCount = new UsageCounter("RpcApi.Listener.{0}", new object[1]
        {
            (object) Process.GetCurrentProcess().Id
        });
        private static readonly FunctionPtr<RpcServerApi.RPC_IF_CALLBACK_FN> hAuthCall = new FunctionPtr<RpcServerApi.RPC_IF_CALLBACK_FN>(new RpcServerApi.RPC_IF_CALLBACK_FN(RpcServerApi.AuthCall));
        private delegate int RPC_IF_CALLBACK_FN(IntPtr Interface, IntPtr Context);
        private bool _isListening;
        private readonly RpcHandle _handle;
        public readonly Guid IID;
        private int _maxCalls;
        private RpcServerApi.RpcExecuteHandler _handler;
        public delegate byte[] RpcExecuteHandler(IRpcClientInfo client, byte[] input);

        [DllImport("Rpcrt4.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcServerUnregisterIf(IntPtr IfSpec, IntPtr MgrTypeUuid, uint WaitForCallsToComplete);

        [DllImport("Rpcrt4.dll", EntryPoint = "RpcServerUseProtseqEpW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcServerUseProtseqEp(string Protseq, int MaxCalls, string Endpoint, IntPtr SecurityDescriptor);

        [DllImport("Rpcrt4.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcServerRegisterIf2(IntPtr IfSpec, IntPtr MgrTypeUuid, IntPtr MgrEpv, int Flags, int MaxCalls, int MaxRpcSize, IntPtr hProc);

        [DllImport("Rpcrt4.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcServerRegisterIf(IntPtr IfSpec, IntPtr MgrTypeUuid, IntPtr MgrEpv);

        [DllImport("Rpcrt4.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcMgmtStopServerListening(IntPtr ignore);

        [DllImport("Rpcrt4.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcMgmtWaitServerListen();

        [DllImport("Rpcrt4.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern RpcError RpcServerListen(uint MinimumCallThreads, int MaxCalls, uint DontWait);

        public virtual byte[] Execute(IRpcClientInfo client, byte[] input)
        {
            RpcServerApi.RpcExecuteHandler rpcExecuteHandler = this._handler;
            if (rpcExecuteHandler != null)
                return rpcExecuteHandler(client, input);
            return (byte[])null;
        }

        public RpcServerApi(Guid iid, int maxCalls, int maxRequestBytes, bool allowAnonTcp)
        {
            this.IID = iid;
            this._maxCalls = maxCalls;
            this._handle = (RpcHandle)new RpcServerApi.RpcServerHandle();
            if (Guid.Empty.Equals(iid)) return;
            RpcServerApi.ServerRegisterInterface(this._handle, this.IID, new RpcExecute(this.RpcEntryPoint), maxCalls, maxRequestBytes, allowAnonTcp);
        }

        public RpcServerApi(Guid iid)
          : this(iid, (int)byte.MaxValue, -1, false)
        {
        }

        public void Dispose()
        {
            this._handler = (RpcServerApi.RpcExecuteHandler)null;
            this.StopListening();
            this._handle.Dispose();
        }

        public void StartListening()
        {
            if (this._isListening)
                return;
            RpcServerApi._listenerCount.Increment<int>(new Action<int>(RpcServerApi.ServerListen), this._maxCalls);
            this._isListening = true;
        }

        public void StopListening()
        {
            if (!this._isListening)
                return;
            this._isListening = false;
            RpcServerApi._listenerCount.Decrement(new ThreadStart(RpcServerApi.ServerStopListening));
        }

        public bool AddProtocol(RpcProtseq protocol, string endpoint, int maxCalls)
        {
            this._maxCalls = Math.Max(this._maxCalls, maxCalls);
            return RpcServerApi.ServerUseProtseqEp(protocol, maxCalls, endpoint);
        }

        public event RpcServerApi.RpcExecuteHandler OnExecute
        {
            add
            {
                lock (this)
                {
                    this._handler = value;
                }
            }
            remove
            {
                lock (this)
                {
                    this._handler = (RpcServerApi.RpcExecuteHandler)null;
                }
            }
        }

        private uint RpcEntryPoint(IntPtr clientHandle, uint szInput, IntPtr input, out uint szOutput, out IntPtr output)
        {
            output = IntPtr.Zero;
            szOutput = 0U;
            try
            {
                byte[] numArray = new byte[szInput];
                Marshal.Copy(input, numArray, 0, numArray.Length);
                byte[] source;
                using (RpcClientInfo rpcClientInfo = new RpcClientInfo(clientHandle))
                    source = this.Execute((IRpcClientInfo)rpcClientInfo, numArray);
                if (source == null)
                    return 1715;
                szOutput = (uint)source.Length;
                output = RpcApi.Alloc(szOutput);
                Marshal.Copy(source, 0, output, source.Length);
                return 0;
            }
            catch (Exception ex)
            {
                RpcApi.Free(output);
                output = IntPtr.Zero;
                szOutput = 0U;
                return 2147500037;
            }
        }
        
        private static void ServerListen(int maxCalls)
        {
            RpcError errorCode = RpcServerApi.RpcServerListen(1U, maxCalls, 1U);
            if (errorCode == RpcError.RPC_S_ALREADY_LISTENING)
                errorCode = RpcError.RPC_S_OK;
            RpcException.Assert(errorCode);
        }

        private static bool ServerUseProtseqEp(RpcProtseq protocol, int maxCalls, string endpoint)
        {
            RpcError errorCode = RpcServerApi.RpcServerUseProtseqEp(protocol.ToString(), maxCalls, endpoint, IntPtr.Zero);
            if (errorCode != RpcError.RPC_S_DUPLICATE_ENDPOINT)
            {
                RpcException.Assert(errorCode);
            }

            return errorCode == RpcError.RPC_S_OK;
        }

        private static int AuthCall(IntPtr Interface, IntPtr Context)
        {
            return 0;
        }

        private static void ServerStopListening()
        {
            int num1 = (int)RpcServerApi.RpcMgmtStopServerListening(IntPtr.Zero);
            int num2 = (int)RpcServerApi.RpcMgmtWaitServerListen();
        }

        private static void ServerRegisterInterface(RpcHandle handle, Guid iid, RpcExecute fnExec, int maxCalls, int maxRequestBytes, bool allowAnonTcp)
        {
            int Flags = 0;
            IntPtr hProc = IntPtr.Zero;
            if (allowAnonTcp)
            {
                Flags = 16;
                hProc = RpcServerApi.hAuthCall.Handle;
            }
            Ptr<RPC_SERVER_INTERFACE> ptr = MIDL_SERVER_INFO.Create(handle, iid, RpcApi.TYPE_FORMAT, RpcApi.FUNC_FORMAT, fnExec);
            if (!allowAnonTcp && maxRequestBytes < 0)
            {
                RpcException.Assert(RpcServerApi.RpcServerRegisterIf(ptr.Handle, IntPtr.Zero, IntPtr.Zero));
            }
            else
            {
                RpcException.Assert(RpcServerApi.RpcServerRegisterIf2(ptr.Handle, IntPtr.Zero, IntPtr.Zero, Flags, maxCalls <= 0 ? (int)byte.MaxValue : maxCalls, maxRequestBytes <= 0 ? 81920 : maxRequestBytes, hProc));
            }

            handle.Handle = ptr.Handle;
        }

        private class RpcServerHandle : RpcHandle
        {
            protected override void DisposeHandle(ref IntPtr handle)
            {
                if (!(handle != IntPtr.Zero))
                    return;
                int num = (int)RpcServerApi.RpcServerUnregisterIf(handle, IntPtr.Zero, 1U);
                handle = IntPtr.Zero;
            }
        }
    }
}
