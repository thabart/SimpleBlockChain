using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.Core.Nodes;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.WalletUI.Events;
using SimpleBlockChain.WalletUI.UserControls;
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
        private const string ADR = "192.168.76.132";
        private const int REFRESH_INFORMATION_INTERVAL = 5000;
        private readonly WalletInformation _walletInformation;
        private readonly BlockChainInformation _blockChainInformation;
        private readonly MemoryPoolInformation _memoryPoolInformation;
        private WalletPageViewModel _viewModel;
        private NodeLauncher _nodeLauncher;
        private Timer _timer;
        private readonly AutoResetEvent _autoEvent = null;
        private readonly BackgroundWorker _refreshUiBackgroundWorker;
        private readonly INodeLauncherFactory _nodeLauncherFactory;

        public WalletPage(INodeLauncherFactory nodeLauncherFactory, WalletInformation walletInformation, BlockChainInformation blockChainInformation, MemoryPoolInformation memoryPoolInformation)
        {
            _walletInformation = walletInformation;
            _blockChainInformation = blockChainInformation;
            _memoryPoolInformation = memoryPoolInformation;
            _nodeLauncherFactory = nodeLauncherFactory;
            _autoEvent = new AutoResetEvent(false);
            _refreshUiBackgroundWorker = new BackgroundWorker();
            _refreshUiBackgroundWorker.DoWork += RefreshUi;
            RegisterEvts();
            InitializeComponent();
        }

        private void RegisterEvts()
        {
            Loaded += Load;
            Unloaded += Unload;
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            Init();
            OpenNetwork(Networks.MainNet);
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            Destroy();
        }

        private void Init()
        {
            _viewModel = new WalletPageViewModel();
            _viewModel.NetworkSwitchEvt += NetworkSwitch;
            _viewModel.RefreshBlockChainEvt += RefreshBlockChain;
            DataContext = _viewModel;
            var walletInformationTab = new TabItem
            {
                Header = "Wallet",
                Content = _walletInformation
            };
            var blocksTab = new TabItem
            {
                Header = "Blocks",
                Content = _blockChainInformation
            };
            var memoryPoolTab = new TabItem
            {
                Header = "Memory POOL",
                Content = _memoryPoolInformation
            };
            this.tabControl.Items.Add(walletInformationTab);
            this.tabControl.Items.Add(blocksTab);
            this.tabControl.Items.Add(memoryPoolTab);
        }

        private void NetworkSwitch(object sender, NetworkEventHandler e)
        {
            OpenNetwork(e.GetNework());
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
            var ipBytes = IPAddress.Parse(ADR).MapToIPv6().GetAddressBytes();
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
            WalletStore.Instance().Switch(_viewModel.IsTestNetChecked ? Networks.TestNet : Networks.MainNet);
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
            RefreshNbBlocks();
            _walletInformation.Refresh();
            _blockChainInformation.Refresh(_viewModel.NbBlocks);
            _memoryPoolInformation.Refresh();
        }

        private void RefreshNbBlocks()
        {
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                return;
            }

            var rpcClient = new RpcClient(authenticatedWallet.Network);
            rpcClient.GetBlockCount().ContinueWith((r) =>
            {
                try
                {
                    var  nb = r.Result;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _viewModel.NbBlocks = nb;
                    });
                }
                catch (AggregateException ex)
                {

                }
            });
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
                _timer.Dispose(_autoEvent);
                _timer = null;
            }

            _viewModel.IsConnected = false;
            _viewModel.NbBlocks = 0;
            _walletInformation.Reset();
            _blockChainInformation.Reset();
            _memoryPoolInformation.Reset();
        }

        private void Destroy()
        {
            Disconnect();
            _viewModel.NetworkSwitchEvt -= NetworkSwitch;
            _viewModel.RefreshBlockChainEvt -= RefreshBlockChain;
            _viewModel = null;
            this.tabControl.Items.Clear();
        }
    }
}
