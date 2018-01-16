using MahApps.Metro.Controls;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.WalletUI.Events;
using SimpleBlockChain.WalletUI.Stores;
using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.UserControls
{
    public partial class BlockChainInformation : UserControl
    {
        private BlockChainInformationViewModel _viewModel;

        public BlockChainInformation()
        {
            Loaded += Load;
            InitializeComponent();
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
            {
                _viewModel = new BlockChainInformationViewModel();
                DataContext = _viewModel;
                _viewModel.SelectBlockEvt += DisplayBlock;
                _viewModel.RefreshEvt += Refresh;
            }
        }

        private void Refresh(object sender, EventArgs e)
        {
            if (_viewModel == null) { return; }
            RefreshBlocks();
        }

        private void DisplayBlock(object sender, BlockEventArgs e)
        {
            var walletStore = WalletStore.Instance();
            var flyout = new BlockFlyoutPage(e.Data, walletStore.GetAuthenticatedWallet().Network);
            MainWindowStore.Instance().DisplayFlyout(flyout, Position.Right, 400);
        }

        private void RefreshBlocks()
        {
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                return;
            }

            var rpcClient = new RpcClient(authenticatedWallet.Network);
            var startIndex = 0;
            var lastIndex = WalletPageStore.Instance().NbBlocks;
            var waitBlockHashes = new List<Task<IEnumerable<byte>>>();
            for (var i = startIndex; i < lastIndex; i++)
            {
                waitBlockHashes.Add(rpcClient.GetBlockHash(i));
            }

            Task.WhenAll(waitBlockHashes.ToArray()).ContinueWith((r) =>
            {
                var waitAllBlocks = new List<Task<Block>>();
                try
                {
                    var lstBHash = r.Result.ToList();
                    foreach (var bHash in lstBHash)
                    {
                        if (bHash == null) { continue; }
                        waitAllBlocks.Add(rpcClient.GetBlock(bHash));
                    }
                }
                catch (AggregateException) { }

                Task.WhenAll(waitAllBlocks.ToArray()).ContinueWith((b) =>
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _viewModel.Blocks.Clear();
                        });
                        var blocks = b.Result;
                        foreach (var block in blocks)
                        {
                            if (block == null) { continue; }
                            var blockHash = block.GetHashHeader().ToHexString();
                            var previousHash = block.BlockHeader.PreviousBlockHeader.ToHexString();
                            var nbTransactions = block.Transactions.Count();
                            var fees = block.GetTotalFees();
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
                    }
                    catch (AggregateException) { }
                });
            });
        }
    }
}
