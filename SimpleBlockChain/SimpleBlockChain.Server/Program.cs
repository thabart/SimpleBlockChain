using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Interop;
using System;

namespace SimpleBlockChain.Server
{
    class Program
    {
        private static string _host = "localhost";

        static void Main(string[] args)
        {
            LaunchServer();
            Console.WriteLine("Ping the PEER ?");
            Console.ReadLine();
            SendPing();

        }

        private static void LaunchServer()
        {
            var iid = Constants.InterfaceId;
            var server = new RpcServerApi(iid, 1234, -1, true);
            server.AddProtocol(RpcProtseq.ncacn_ip_tcp, Core.Constants.Ports.MainNet, 5);
            server.StartListening();
            Console.WriteLine("Is listening");
            server.OnExecute += delegate (IRpcClientInfo client, byte[] arg)
            {
                return new byte[0];
            };
        }

        private static void SendPing()
        {
            var iid = Constants.InterfaceId;
            using (RpcClientApi client = new RpcClientApi(iid, RpcProtseq.ncacn_ip_tcp, _host, Core.Constants.Ports.MainNet))
            {
                var nonce = GetNonce();
                var pingMessage = new PingMessage(nonce, Core.Networks.MainNet);
                var payload = pingMessage.Serialize();
                byte[] response = client.Execute(payload);
            }
        }

        private static ulong GetNonce()
        {
            var random = new Random();
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            short hi = (short)random.Next(4, 0x10000);
            buffer[7] = (byte)(hi >> 8);
            buffer[6] = (byte)hi;
            return (ulong)BitConverter.ToInt64(buffer, 0);
        }
    }
}
