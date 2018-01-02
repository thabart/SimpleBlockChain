using MahApps.Metro.Controls.Dialogs;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Aggregates;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Rpc.Parameters;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using SimpleBlockChain.WalletUI.Stores;
using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.UserControls
{
    public partial class WalletInformation : UserControl
    {
        private readonly IScriptBuilder _scriptBuilder;
        private readonly ITransactionHelper _transactionHelper;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionBuilder _transactionBuilder;
        private WalletInformationViewModel _viewModel;
        private object _lock = new object();

        public WalletInformation(IScriptBuilder scriptBuilder, ITransactionHelper transactionHelper, 
            IWalletRepository walletRepository, ITransactionBuilder transactionBuilder)
        {
            _scriptBuilder = scriptBuilder;
            _transactionHelper = transactionHelper;
            _walletRepository = walletRepository;
            _transactionBuilder = transactionBuilder;
            InitializeComponent();
            Loaded += Load;
            Unloaded += Unload;
        }

        public void Refresh()
        {
            if (_viewModel == null) { return; }
            RefreshMoney();
            RefreshBalance();
        }

        public void Reset()
        {
            if (_viewModel == null) { return; }
            _viewModel.Reset();
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            _viewModel = new WalletInformationViewModel();
            _viewModel.SendMoneyEvt += SendMoney;
            DataContext = _viewModel;
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            Destroy();
        }

        private void SendMoney(object sender, EventArgs e)
        {
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                MainWindowStore.Instance().DisplayError("You're not authenticated");
                return;
            }

            var receiverValue = _viewModel.SendValue;
            var addr = _viewModel.SendAddress;
            var selectedTransaction = _viewModel.SelectedTransaction;
            if (selectedTransaction == null)
            {
                MainWindowStore.Instance().DisplayError("A transaction must be selected");
                return;
            }

            if (receiverValue > selectedTransaction.Amount)
            {
                return;
            }

            var txFee = _transactionHelper.GetMinFee();
            var senderValue = selectedTransaction.Amount - receiverValue - txFee;
            var walletAddr = authenticatedWallet.Addresses.FirstOrDefault(a => a.Hash == selectedTransaction.Hash);
            if (walletAddr == null)
            {
                MainWindowStore.Instance().DisplayError("The selected tranasction is not linked to your wallet");
                return;
            }

            BlockChainAddress bcAddr = null;
            try
            {
                bcAddr = BlockChainAddress.Deserialize(addr);
            }
            catch (Exception)
            {
                MainWindowStore.Instance().DisplayError("The address is not correct");
            }

            if (bcAddr == null)
            {
                MainWindowStore.Instance().DisplayError("The address is not correct");
                return;
            }

            var newKey = CreateNewAddress();
            var kh = new BigInteger(newKey.GetPublicKeyHashed());
            var script = _scriptBuilder.New()
                .AddToStack(walletAddr.Key.GetSignature())
                .AddToStack(walletAddr.Key.GetPublicKey())
                .Build();
            var pp = walletAddr.Key.GetPublicKeyHashed();
            var senderSript = _scriptBuilder.New() // SEND MONEY TO MY WALLET.
                .AddOperation(OpCodes.OP_DUP)
                .AddOperation(OpCodes.OP_HASH160)
                .AddToStack(newKey.GetPublicKeyHashed())
                .AddOperation(OpCodes.OP_EQUALVERIFY)
                .AddOperation(OpCodes.OP_CHECKSIG)
                .Build();
            var receiverScript = _scriptBuilder.New() // SEND MONEY TO THE SELLER.
                .AddOperation(OpCodes.OP_DUP)
                .AddOperation(OpCodes.OP_HASH160)
                .AddToStack(bcAddr.PublicKeyHash)
                .AddOperation(OpCodes.OP_EQUALVERIFY)
                .AddOperation(OpCodes.OP_CHECKSIG)
                .Build();
            var txBuilder = _transactionBuilder.NewNoneCoinbaseTransaction()
                .Spend(selectedTransaction.TxId.FromHexString(), (uint)selectedTransaction.Vout, script.Serialize())
                .AddOutput((long)receiverValue, receiverScript);
            if (senderValue > 0)
            {
                txBuilder.AddOutput((long)senderValue, senderSript);
            }

            var tx = txBuilder.Build();
            var s = tx.Serialize().Count();
            var rpcClient = new RpcClient(authenticatedWallet.Network);
            rpcClient.SendRawTransaction(tx).ContinueWith((r) =>
            {
                try
                {
                    var res = r.Result;
                }
                catch (AggregateException ex)
                {
                    var exx = ex.InnerExceptions;
                }
            });
        }

        private void RefreshMoney()
        {
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                return;
            }

            var rpcClient = new RpcClient(authenticatedWallet.Network);
            rpcClient.GetUnspentTransactions(new GetUnspentTransactionsParameter()).ContinueWith((r) =>
            {
                lock (_lock)
                {
                    try
                    {
                        var unspentTransactions = r.Result.Where(t => t.Spendable);
                        if (unspentTransactions != null)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                var selectedTransaction = _viewModel.SelectedTransaction;
                                var transactionsToUpdate = _viewModel.Transactions.Where(tvm => unspentTransactions.Any(utxo => tvm.TxId == utxo.TxId && tvm.Vout == utxo.Vout)).ToList();
                                var transactionsToRemove = _viewModel.Transactions.Where(tvm => unspentTransactions.All(utxo => tvm.TxId != utxo.TxId && tvm.Vout != utxo.Vout)).ToList();
                                foreach (var txUpdate in transactionsToUpdate)
                                {
                                    var tr = unspentTransactions.First(u => u.TxId == txUpdate.TxId && u.Vout == txUpdate.Vout);
                                    txUpdate.Amount = tr.Amount;
                                    txUpdate.DisplayName = string.Format("{0} : {1}", tr.Amount, tr.TxId);
                                }

                                foreach (var txRemove in transactionsToRemove)
                                {
                                    _viewModel.Transactions.Remove(txRemove);
                                }

                                var transactionsToAdd = unspentTransactions.Where(utxo => _viewModel.Transactions.All(tvm => tvm.TxId != utxo.TxId && tvm.Vout != utxo.Vout)).ToList();
                                foreach (var transactionToAdd in transactionsToAdd)
                                {
                                    var txVm = new TransactionViewModel(transactionToAdd.TxId, transactionToAdd.Vout, transactionToAdd.Amount, transactionToAdd.Address, transactionToAdd.Confirmations);
                                    _viewModel.Transactions.Add(txVm);
                                }

                                _viewModel.Amount = unspentTransactions.Where(t => t.Confirmations > 0).Sum(t => t.Amount);
                            });
                        }

                    }
                    catch (AggregateException ex)
                    {
                        // TODO : Display loading message.
                    }
                }
            });
        }

        private void RefreshBalance()
        {
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                return;
            }

            var rpcClient = new RpcClient(authenticatedWallet.Network);
            rpcClient.GetUnconfirmedBalance().ContinueWith((r) =>
            {
                try
                {
                    var balance = (int)r.Result;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _viewModel.Balance = balance;
                    });
                }
                catch (AggregateException ex)
                {

                }
            });
        }

        private Key CreateNewAddress()
        {
            var walletStore = WalletStore.Instance();
            var authenticatedWallet = walletStore.GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                return null;
            }

            var key = Key.Genererate();
            var blockChainAdr = new BlockChainAddress(ScriptTypes.P2PKH, authenticatedWallet.Network, key);
            authenticatedWallet.Addresses.Add(new WalletAggregateAddress
            {
                Hash = blockChainAdr.GetSerializedHash(),
                Key = key,
                Network = authenticatedWallet.Network
            });
            var password = walletStore.GetPassword();
            _walletRepository.Update(authenticatedWallet, walletStore.GetPassword());
            return key;
        }

        private void Destroy()
        {
            if (_viewModel == null) { return; }
            _viewModel.SendMoneyEvt -= SendMoney;
            _viewModel = null;
        }
    }
}
