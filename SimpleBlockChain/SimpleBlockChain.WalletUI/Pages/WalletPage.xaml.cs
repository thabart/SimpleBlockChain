using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Nodes;
using SimpleBlockChain.WalletUI.Events;
using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Pages
{
    public partial class WalletPage : Page
    {
        private readonly WalletPageViewModel _viewModel;
        private NodeLauncher _nodeLauncher;

        public WalletPage()
        {
            _viewModel = new WalletPageViewModel();
            _viewModel.SendMoneyEvt += SendMoney;
            _viewModel.NetworkSwitchEvt += NetworkSwitch;
            Loaded += Load;
            Unloaded += Unload;
            InitializeComponent();
            DataContext = _viewModel;
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            OpenNetwork(Networks.MainNet);
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        private void NetworkSwitch(object sender, NetworkEventHandler e)
        {
            OpenNetwork(e.GetNework());
        }

        private void SendMoney(object sender, EventArgs e)
        {

        }

        private void OpenNetwork(Networks network)
        {
            Disconnect();
            var ipBytes = IPAddress.Parse("192.168.76.132").MapToIPv6().GetAddressBytes();
            _nodeLauncher = new NodeLauncher(network, ServiceFlags.NODE_NONE);
            _nodeLauncher.LaunchP2PNode(ipBytes);
            _nodeLauncher.LaunchRPCNode();
            _nodeLauncher.ConnectP2PEvent += ConnectP2PNetwork;
            _nodeLauncher.DisconnectP2PEvent += DisconnectP2PNetwork;
        }

        private void ConnectP2PNetwork(object sender, EventArgs e)
        {
            _viewModel.IsConnected = true;
        }

        private void DisconnectP2PNetwork(object sender, EventArgs e)
        {
            _viewModel.IsConnected = false;
        }

        private void Disconnect()
        {
            if (_nodeLauncher != null)
            {
                _nodeLauncher.Dispose();
                _nodeLauncher = null;
            }
        }
    }
}
