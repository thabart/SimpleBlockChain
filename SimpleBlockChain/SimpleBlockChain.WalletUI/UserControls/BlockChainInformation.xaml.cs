using MahApps.Metro.Controls;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.WalletUI.Events;
using SimpleBlockChain.WalletUI.Stores;
using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.UserControls
{
    public partial class BlockChainInformation : UserControl
    {
        private const int BLOCKS_PER_PAGE = 5;
        private int _currentPage = 0;
        private BlockChainInformationViewModel _viewModel;
        private int _currentNbBlocks = 0;

        public BlockChainInformation()
        {
            Loaded += Load;
            Unloaded += Unload;
            InitializeComponent();
        }

        public void Refresh(int nbBlocks)
        {
            if (_viewModel == null) { return; }
            _currentNbBlocks = nbBlocks;
            RefreshBlocks();
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            Destroy();
        }

        private void Init()
        {
            _viewModel = new BlockChainInformationViewModel();
            DataContext = _viewModel;
            _viewModel.NextPageEvt += NextPage;
            _viewModel.PreviousPageEvt += PreviousPage;
            _viewModel.SelectBlockEvt += DisplayBlock;
        }

        private void DisplayBlock(object sender, BlockEventArgs e)
        {
            var walletStore = WalletStore.Instance();
            var flyout = new BlockFlyoutPage(e.Data, walletStore.GetAuthenticatedWallet().Network);
            MainWindowStore.Instance().DisplayFlyout(flyout, Position.Right, 400);
        }

        private void NextPage(object sender, EventArgs e)
        {
            _currentPage += 1;
            RefreshBlocks();
        }

        private void PreviousPage(object sender, EventArgs e)
        {
            _currentPage -= 1;
            RefreshBlocks();
        }

        private void RefreshBlocks()
        {
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                return;
            }

            var rpcClient = new RpcClient(authenticatedWallet.Network);
            var startIndex = _currentPage * BLOCKS_PER_PAGE;
            var lastIndex = startIndex + (BLOCKS_PER_PAGE - 1);
            if (lastIndex > (_currentNbBlocks - 1))
            {
                lastIndex = _currentNbBlocks;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                _viewModel.Blocks.Clear();
            });
            for (var i = startIndex; i < lastIndex; i++)
            {
                rpcClient.GetBlockHash(i).ContinueWith((r) =>
                {
                    try
                    {
                        var bHash = r.Result;
                        if (bHash == null) { return; }
                        rpcClient.GetBlock(bHash).ContinueWith((sr) =>
                        {
                            try
                            {
                                Block b = sr.Result;
                                if (b == null)
                                {
                                    return;
                                }

                                var blockHash = b.GetHashHeader().ToHexString();
                                var previousHash = b.BlockHeader.PreviousBlockHeader.ToHexString();
                                var nbTransactions = b.Transactions.Count();
                                var fees = b.GetTotalFees();
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    _viewModel.Blocks.Add(new BlockViewModel
                                    {
                                        Fees = fees,
                                        Hash = blockHash,
                                        PreviousHash = previousHash
                                    });
                                });
                            }
                            catch (AggregateException ex) { }
                        });
                    }
                    catch (AggregateException ex) { }
                });
            }
        }

        private void Destroy()
        {
            if (_viewModel == null) { return; }
            _viewModel.NextPageEvt -= NextPage;
            _viewModel.PreviousPageEvt -= PreviousPage;
            _viewModel.SelectBlockEvt -= DisplayBlock;
            _viewModel = null;
        }
    }
}
