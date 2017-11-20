﻿using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Crypto;
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
            { 3, SendVersion },
            { 4, Quit }
        };
        private static RpcServerApi _server;
        private static RpcClientApi _client;

        static void Main(string[] args)
        {
            // CreateTransaction();
            LaunchServer();
            LaunchClient();
            DisplayMenu();
        }

        private static void DisplayMenu()
        {
            Console.WriteLine("1. Send ping ?");
            Console.WriteLine("2. Send addr ?");
            Console.WriteLine("3. Send version ?");
            Console.WriteLine("4. Exit ?");
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

        private static void SendVersion() // Send the version.
        {
            var ipv6 = GetIpv4();
            var transmittingNode = new IpAddress(DateTime.UtcNow, ServiceFlags.NODE_NONE, ipv6, ushort.Parse(Core.Constants.Ports.MainNet));
            var receivingNode = new IpAddress(DateTime.UtcNow, ServiceFlags.NODE_NETWORK, ipv6, ushort.Parse(Core.Constants.Ports.MainNet));
            var nonce = GetNonce();
            var versionMessage = new VersionMessage(transmittingNode, receivingNode, nonce, string.Empty, 0, false, Networks.MainNet);
            byte[] response = _client.Execute(versionMessage.Serialize());
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

        private static void CreateAddress()
        {
            var key = new Key(); // BOB generates a new key.
            var blockChainAddress = new BlockChainAddress(ScriptTypes.P2PKH, Networks.MainNet, key); // BOB generate a new block chain address.
        }

        private static string CreateTransaction()
        {
            // https://bitcoin.org/en/developer-guide#transactions
            // Scenario : Bob spends alice's transaction.
            var key = new Key();
            var blockChainAddress = new BlockChainAddress(ScriptTypes.P2PKH, Networks.MainNet, key); // BOB generates a bitcoin address.
            string adr = blockChainAddress.GetSerializedHash();
            Console.WriteLine($"BOB's address is {adr}");
            var receivedBlockChainAddress = BlockChainAddress.Deserialize(adr); // ALICE parses the bitcoin address.
            var publicKeyHash = receivedBlockChainAddress.PublicKeyHash;
            if (receivedBlockChainAddress.Type == ScriptTypes.P2PKH)
            {
                var transactionBuilder = new TransactionBuilder();
                var script = Script.CreateP2PKHScript(publicKeyHash);
                transactionBuilder.AddOutput(49, script);
                var transaction = transactionBuilder.Build();
                var serializedTransaction = transaction.Serialize(); // ALICE creates the first transaction - only one output and no input.
                Console.WriteLine("The first transaction has been braodcast by ALICE");

                var deserializedTransaction = Transaction.Deserialize(serializedTransaction); // BOB receives the transaction and deserialize it : SPEND Unspent Transaction Ouput (UTXO).
                Console.WriteLine("Bob received the first transaction and create a new one");
                var firstTransactionOutput = deserializedTransaction.TransactionOut.First();
                var transactionInputBuilder = new TransactionInputBuilder();
                transactionInputBuilder.AddOutput(deserializedTransaction.GetTxId(), 0);
                transactionBuilder.New()
                    .AddOutput(firstTransactionOutput.Value, firstTransactionOutput.Script)
                    .AddInput(transactionInputBuilder.Create());
                switch(firstTransactionOutput.Script.Type)
                {
                    case ScriptTypes.P2PKH:
                        var stack = firstTransactionOutput.Script.ScriptRecords.First(sr => sr.Type == ScriptRecordType.Stack);
                        var publicKey = key.GetPublicKey();
                        var tmpSecondTransaction = transactionBuilder.Build();
                        var tmpSecondTransactionPayload = tmpSecondTransaction.Serialize();
                        var signature = key.Sign(tmpSecondTransactionPayload);
                        transactionInputBuilder.AddSignatureScript(signature, publicKey);
                        break;
                }

                var secondTransactionPayload = transactionBuilder.Build().Serialize();
                string s = "";
            }
            // Create the P2PKH transaction.

            return null;
        }
    }
}
