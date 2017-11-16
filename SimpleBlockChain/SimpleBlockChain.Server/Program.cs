using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Launchers;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Parsers;
using SimpleBlockChain.Core.Transactions;
using SimpleBlockChain.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

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
        private static RpcClientApi _client;

        static void Main(string[] args)
        {
            CreateTransaction();
            /*
            LaunchServer();
            LaunchClient();
            DisplayMenu();
            */
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
            _client.Dispose();
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
            var iid = Interop.Constants.InterfaceId;
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
                if (response == null)
                {
                    return new byte[0];
                }

                return response.Serialize();
            };
        }

        private static void LaunchClient()
        {
            var iid = Interop.Constants.InterfaceId;
            _client = new RpcClientApi(iid, RpcProtseq.ncacn_ip_tcp, _host, Core.Constants.Ports.MainNet);
        }

        private static void SendPing() // Send ping.
        {
            var nonce = GetNonce();
            var pingMessage = new PingMessage(nonce, Core.Networks.MainNet);
            var payload = pingMessage.Serialize();
            byte[] response = _client.Execute(payload);
            string s = "";
        }

        private static void SendAddr() // Send my addr.
        {
            var ipv6 = GetIpv4();      
            var addrMessage = new AddrMessage(new CompactSize { Size = 1 }, Core.Networks.MainNet);
            addrMessage.IpAddresses.Add(new IpAddress(DateTime.UtcNow, ServiceFlags.NODE_NETWORK, ipv6, ushort.Parse(Core.Constants.Ports.MainNet)));
            var payload = addrMessage.Serialize();
            byte[] response = _client.Execute(payload);
        }

        private static byte[] GetIpV6() // Get local IPV6 address.
        {
            var hostName = Dns.GetHostName();
            var ipEntry = Dns.GetHostEntry(hostName);
            var addr = ipEntry.AddressList;
            var ipv6Addr = addr.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetworkV6);
            if (ipv6Addr == null)
            {
                return null;
            }

            return ipv6Addr.GetAddressBytes();
        }

        private static byte[] GetIpv4() // Get IPV4 transformed into IPV6 format.
        {
            var hostName = Dns.GetHostName();
            var ipEntry = Dns.GetHostEntry(hostName);
            var addr = ipEntry.AddressList;
            var ipv4Addr = addr.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            if (ipv4Addr == null)
            {
                return null;
            }

            return ipv4Addr.MapToIPv6().GetAddressBytes();
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

        private static string CreateTransaction()
        {
            // https://bitcoin.org/en/developer-guide#transactions
            // Scenario : Bob spends alice's transaction.
            var blockChainAddress = new BlockChainAddress(ScriptTypes.P2PKH, Networks.MainNet); // Bob generates a bitcoin address.
            blockChainAddress.New();
            string adr = blockChainAddress.GetAddress();
            Console.WriteLine($"BOB's address is {adr}");
            var receivedBlockChainAddress = BlockChainAddress.Parse(adr); // Alice parse the bitcoin address.
            var publicKeyHash = receivedBlockChainAddress.PublicKeyHash;
            if (receivedBlockChainAddress.Type == ScriptTypes.P2PKH)
            {
                var transactionBuilder = new TransactionBuilder();
                var scriptBuilder = new ScriptBuilder();
                var script = scriptBuilder.CreateP2PKHScript(publicKeyHash);
                transactionBuilder.AddOutput(49, script);
                var transaction = transactionBuilder.Build();
                var serializedTransaction = transaction.Serialize(); // Alice creates the first transaction - only one output and no input.

                // Bob receives the transaction and deserialize it : SPEND Unspent Transaction Ouput (UTXO).
            }
            // Create the P2PKH transaction.

            return null;
        }
    }
}
