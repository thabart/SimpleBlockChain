using MahApps.Metro.Controls;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Transactions;
using SimpleBlockChain.WalletUI.Stores;
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
            Loaded += Load;
            Unloaded += Unload;
            InitializeComponent();
        }

        private void Init()
        {
            _viewModel = new BlockFlyoutViewModel();
            _viewModel.OpenTransactionEvt += OpenTransaction;
            DataContext = _viewModel;
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

        private void Load(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            _viewModel.OpenTransactionEvt -= OpenTransaction;
            _viewModel = null;
        }

        private void OpenTransaction(object sender, EventArgs e)
        {
            var selectedTransaction = _viewModel.SelectedTransaction;
            if (selectedTransaction == null)
            {
                return;
            }

            var txId = selectedTransaction.TxId.FromHexString();
            var flyout = new TransactionFlyout(txId, _network);
            MainWindowStore.Instance().DisplayFlyout(flyout, Position.Left, 400);
        }
    }
}
