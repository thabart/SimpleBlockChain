﻿using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Nodes;
using System;
using System.Net;

namespace SimpleBlockChain.Client
{
    class Program
    {
        private static NodeLauncher _nodeLauncher;
        static string testPath = @"C:\Temp\Test";

        static void Main(string[] args)
        {
            Console.WriteLine("==== Welcome to SimpleBlockChain (SEED NODE) ====");
            var network = MenuHelper.ChooseNetwork();
            var ipBytes = IPAddress.Parse(Constants.DNS_IP_ADDRESS).MapToIPv6().GetAddressBytes();
            _nodeLauncher = new NodeLauncher(network, ServiceFlags.NODE_NETWORK);
            var p2pNode = _nodeLauncher.GetP2PNode();
            p2pNode.StartNodeEvent += StartP2PNodeEvent;
            p2pNode.NewMessageEvent += NewP2PMessageEvent;
            _nodeLauncher.ConnectP2PEvent += ConnectP2PEvent;
            _nodeLauncher.DisconnectP2PEvent += DisconnectP2PEvent;
            _nodeLauncher.LaunchP2PNode(ipBytes);
            _nodeLauncher.LaunchRPCNode();
            Console.ReadLine();
        }

        private static void ConnectP2PEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayInformation("Connected to P2P network");
        }

        private static void DisconnectP2PEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayError("Cannot connect to P2P network... Retry in 10 seconds");
        }

        private static void StartP2PNodeEvent(object sender, EventArgs e)
        {
            MenuHelper.DisplayInformation("Node is listening");
            _nodeLauncher.ConnectP2PNetwork();
        }

        private static void NewP2PMessageEvent(object sender, StringEventArgs e)
        {
            MenuHelper.DisplayInformation($"Message {e.Data} arrived");
        }
    }
}
