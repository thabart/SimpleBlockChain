using SimpleBlockChain.Interop;
using System;

namespace SimpleBlockChain.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var iid = Constants.InterfaceId;
            using (RpcClientApi client = new RpcClientApi(iid, RpcProtseq.ncacn_ip_tcp, "localhost", "8801"))
            {
                // client.AuthenticateAs(RpcClientApi.Self);
                byte[] response = client.Execute(new byte[0]);
            }
            Console.ReadLine();
            string s = "";
        }
    }
}
