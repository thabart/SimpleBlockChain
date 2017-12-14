using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Transactions;
using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.UserControls
{
    public partial class BlockFlyoutPage : UserControl
    {
        private IEnumerable<byte> _hash;
        private Networks _network;
        private BlockFlyoutViewModel _viewModel;

        public BlockFlyoutPage(string hash, Networks network)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                throw new ArgumentNullException(nameof(hash));
            }

            _hash = hash.FromHexString();
            _network = network;
            _viewModel = new BlockFlyoutViewModel();
            DataContext = _viewModel;
            Loaded += Load;
            Unloaded += Unload;
            InitializeComponent();
        }

        public BlockFlyoutPage(IEnumerable<byte> hash, Networks network)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            _hash = hash;
            _network = network;
            _viewModel = new BlockFlyoutViewModel();
            DataContext = _viewModel;
            Loaded += Load;
            Unloaded += Unload;
            InitializeComponent();
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            _viewModel = null;
        }

        public void Init()
        {
            var rpcClient = new RpcClient(_network);
            rpcClient.GetBlock(_hash).ContinueWith((r) =>
            {
                try
                {
                    var block = r.Result;
                    if (block == null)
                    {
                        return;
                    }

                    if (block.Transactions != null && !block.Transactions.Any())
                    {
                        return;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _viewModel.Transactions.Clear();
                        foreach (var tx in block.Transactions)
                        {
                            var txId = tx.GetTxId().ToHexString();
                            var record = new BlockTransactionViewModel
                            {
                                TxId = txId,
                                IsCoinBase = tx is CoinbaseTransaction,
                                Value = tx.TransactionOut.Sum(t => t.Value),
                                NbTransactionsOut = tx.TransactionOut.Count()
                            };

                            _viewModel.Transactions.Add(record);
                        }
                    });
                }
                catch (AggregateException ex) { }
            });
        }
    }
}
