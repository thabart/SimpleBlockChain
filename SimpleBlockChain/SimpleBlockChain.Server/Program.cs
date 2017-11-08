using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Launchers;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Parsers;
using SimpleBlockChain.Interop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Server
{
    class Program
    {
        private static string _host = "localhost";
        private static Dictionary<int, Action> _mappingMenuItems = new Dictionary<int, Action>
        {
            { 1, SendPing },
            { 2, SendAddr },
            { 3, Quit }
        };
        private static RpcServerApi _server;

        static void Main(string[] args)
        {
            LaunchServer();
            DisplayMenu();
        }

        private static void DisplayMenu()
        {
            Console.WriteLine("1. Send ping ?");
            Console.WriteLine("2. Send addr ?");
            Console.WriteLine("3. Exit ?");
            var act = EnterNumber();
            act();
            DisplayMenu();
        }

        private static void Quit()
        {
            _server.Dispose();
            Console.WriteLine("bye bye");
            Console.ReadLine();
            Environment.Exit(0);
        }

        private static Action EnterNumber()
        {
            int option;
            if (!int.TryParse(Console.ReadLine(), out option))
            {
                Console.WriteLine("Please enter a correct number");
                return EnterNumber();
            }

            var kvp = _mappingMenuItems.FirstOrDefault(m => m.Key == option);
            if (kvp.Equals(default(KeyValuePair<int, Action>)) && kvp.Value == null)
            {
                return EnterNumber();
            }

            return kvp.Value;
        }

        private static void LaunchServer()
        {
            var iid = Constants.InterfaceId;
            _server = new RpcServerApi(iid, 1234, -1, true);
            _server.AddProtocol(RpcProtseq.ncacn_ip_tcp, Core.Constants.Ports.MainNet, 5);
            _server.StartListening();
            Console.WriteLine("Is listening");
            _server.OnExecute += delegate (IRpcClientInfo client, byte[] arg)
            {
                var messageParser = new MessageParser();
                var messageLauncher = new MessageLauncher();
                var message = messageParser.Parse(arg);
                Console.WriteLine(string.Format("A message has been received {0}", message.GetCommandName()));
                var response =  messageLauncher.Launch(message);
                return response.Serialize();
            };
        }

        private static void SendPing() // Send ping.
        {
            var iid = Constants.InterfaceId;
            using (RpcClientApi client = new RpcClientApi(iid, RpcProtseq.ncacn_ip_tcp, _host, Core.Constants.Ports.MainNet))
            {
                var nonce = GetNonce();
                var pingMessage = new PingMessage(nonce, Core.Networks.MainNet);
                var payload = pingMessage.Serialize();
                byte[] response = client.Execute(payload);
                string s = "";
            }
        }

        private static void SendAddr() // Send addr.
        {
            var iid = Constants.InterfaceId;
            using (RpcClientApi client = new RpcClientApi(iid, RpcProtseq.ncacn_ip_tcp, _host, Core.Constants.Ports.MainNet))
            {
                var addrMessage = new AddrMessage(new CompactSize { Size = 0xffffffffffffffff }, Core.Networks.MainNet);
                var tmp = addrMessage.CompactSize.Serialize();
                string ss = "";
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
