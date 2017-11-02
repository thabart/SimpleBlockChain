using SimpleBlockChain.Interop;
using System;

namespace SimpleBlockChain.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var iid = Constants.InterfaceId;
            using (var server = new RpcServerApi(iid, 1234, -1, true))
            {
                server.AddProtocol(RpcProtseq.ncacn_ip_tcp, "8801", 5);
                server.StartListening();
                Console.WriteLine("Is listening");
                server.OnExecute +=
                delegate (IRpcClientInfo client, byte[] arg)
                {
                    string tmp = "";
                    return new byte[0];
                };
                Console.ReadLine();
            }
        }
    }
}
