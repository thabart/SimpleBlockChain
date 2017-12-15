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
    public partial class TransactionFlyout : UserControl
    {
        private readonly IEnumerable<byte> _txId;
        private readonly Networks _network;
        private TransactionFlyoutViewModel _viewModel;

        public TransactionFlyout(IEnumerable<byte> txId, Networks network)
        {
            _txId = txId;
            _network = network;
            InitializeComponent();
            Loaded += Load;
            Unloaded += Unload;
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            _viewModel = new TransactionFlyoutViewModel();
            DataContext = _viewModel;
            Init();
        }

        private void Init()
        {
            var rpcClient = new RpcClient(_network);
            rpcClient.GetRawTransaction(_txId).ContinueWith((r) =>
            {
                try
                {
                    var transaction = r.Result;
                    if (transaction == null)
                    {
                        return;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _viewModel.TxId = transaction.GetTxId().ToHexString();
                        _viewModel.Version = transaction.Version;
                        _viewModel.Size = transaction.Serialize().Count();
                        var nonceCoinBase = transaction as NoneCoinbaseTransaction;
                        if (nonceCoinBase != null && nonceCoinBase.TransactionIn != null && nonceCoinBase.TransactionIn.Any())
                        {
                            var firstTransactionIn = nonceCoinBase.TransactionIn.First() as TransactionInNoneCoinbase;
                            if (firstTransactionIn != null && firstTransactionIn.Outpoint != null)
                            {
                                _viewModel.PreviousTxId = firstTransactionIn.Outpoint.Hash.ToHexString();
                            }
                        }

                        if (transaction.TransactionOut != null && transaction.TransactionOut.Any())
                        {
                            foreach(var txOut in transaction.TransactionOut)
                            {
                                _viewModel.TxOuts.Add(new TxOutViewModel
                                {
                                    Index = transaction.TransactionOut.IndexOf(txOut),
                                    Value = txOut.Value,
                                    Script = txOut.Script.Serialize().ToHexString()
                                });
                            }
                        }
                    });                   
                    
                }
                catch (AggregateException ex)
                {

                }
            });
        }
    }
}
