using SimpleBlockChain.Interop;
using System;

namespace SimpleBlockChain
{
    class Program
    {
        static void Main(string[] args)
        {
            var iid = Guid.NewGuid();
            using (var server = new RpcServerApi(iid))
            {
                server.AddProtocol(RpcProtseq.ncacn_np, @"\pipe\testpipename", 5);
                server.StartListening();
                Console.WriteLine("Is listening");
                server.OnExecute +=
                delegate (IRpcClientInfo client, byte[] arg)
                {
                    string tmp = "";
                    return new byte[0];
                };

                using (RpcClientApi client = new RpcClientApi(iid, RpcProtseq.ncacn_np, null, @"\pipe\testpipename"))
                {
                    // client.AuthenticateAs(RpcClientApi.Self);
                    byte[] response = client.Execute(new byte[0]);
                }
                Console.ReadLine();
                string s = "";
            }
        }
    }
}
