using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.Core.Nodes;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.WalletUI.Events;
using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Pages
{
    public partial class WalletPage : Page
    {
        private const int REFRESH_INFORMATION_INTERVAL = 5000;
        private readonly WalletPageViewModel _viewModel;
        private NodeLauncher _nodeLauncher;
        private Timer _timer;
        private readonly AutoResetEvent _autoEvent = null;
        private readonly BackgroundWorker _refreshUiBackgroundWorker;
        private readonly INodeLauncherFactory _nodeLauncherFactory;

        public WalletPage(INodeLauncherFactory nodeLauncherFactory)
        {
            _nodeLauncherFactory = nodeLauncherFactory;
            _autoEvent = new AutoResetEvent(false);
            _refreshUiBackgroundWorker = new BackgroundWorker();
            _refreshUiBackgroundWorker.DoWork += RefreshUi;
            _viewModel = new WalletPageViewModel();
            _viewModel.SendMoneyEvt += SendMoney;
            _viewModel.NetworkSwitchEvt += NetworkSwitch;
            _viewModel.RefreshBlockChainEvt += RefreshBlockChain;
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

        private void RefreshBlockChain(object sender, EventArgs e)
        {
            if (!_viewModel.IsConnected)
            {
                return;
            }

            _nodeLauncher.RefreshBlockChain();
        }

        private void OpenNetwork(Networks network)
        {
            Disconnect();
            var ipBytes = IPAddress.Parse("192.168.76.132").MapToIPv6().GetAddressBytes();
            _nodeLauncher = _nodeLauncherFactory.Build(network, ServiceFlags.NODE_NONE);
            _nodeLauncher.LaunchP2PNode(ipBytes);
            _nodeLauncher.LaunchRPCNode();
            _nodeLauncher.ConnectP2PNetwork();
            _nodeLauncher.ConnectP2PEvent += ConnectP2PNetwork;
            _nodeLauncher.DisconnectP2PEvent += DisconnectP2PNetwork;
        }

        private void ConnectP2PNetwork(object sender, EventArgs e)
        {
            _nodeLauncher.RefreshBlockChain();
            _timer = new Timer(TimerElapsed, _autoEvent, REFRESH_INFORMATION_INTERVAL, REFRESH_INFORMATION_INTERVAL);
            _viewModel.IsConnected = true;
        }

        private void TimerElapsed(object sender)
        {
            if (!_refreshUiBackgroundWorker.IsBusy)
            {
                _refreshUiBackgroundWorker.RunWorkerAsync();
            }
        }

        private void RefreshUi(object sender, DoWorkEventArgs e)
        {
            var blockChain = BlockChainStore.Instance().GetBlockChain();
            _viewModel.NbBlocks = blockChain.GetCurrentBlockHeight();
        }

        private void DisconnectP2PNetwork(object sender, EventArgs e)
        {
            if (_timer != null)
            {
                _timer = null;
            }

            _viewModel.IsConnected = false;
            _viewModel.NbBlocks = 0;
        }

        private void Disconnect()
        {
            if (_nodeLauncher != null)
            {
                _nodeLauncher.Dispose();
                _nodeLauncher = null;
            }

            if (_timer != null)
            {
                _timer = null;
            }

            _viewModel.IsConnected = false;
            _viewModel.NbBlocks = 0;
        }
    }
}
