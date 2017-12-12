using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Aggregates;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.Core.Nodes;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Rpc.Parameters;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using SimpleBlockChain.WalletUI.Events;
using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.Pages
{
    public partial class WalletPage : Page
    {
        private const double DEFAULT_TX_SIZE = 500; // TODO : Correctly set the transaction FEE.
        private const int REFRESH_INFORMATION_INTERVAL = 5000;
        private readonly WalletPageViewModel _viewModel;
        private NodeLauncher _nodeLauncher;
        private Timer _timer;
        private readonly AutoResetEvent _autoEvent = null;
        private readonly BackgroundWorker _refreshUiBackgroundWorker;
        private readonly INodeLauncherFactory _nodeLauncherFactory;
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IScriptBuilder _scriptBuilder;
        private readonly IWalletRepository _walletRepository;
        private object _lock = new object();

        public WalletPage(INodeLauncherFactory nodeLauncherFactory, ITransactionBuilder transactionBuilder, IScriptBuilder scriptBuilder, IWalletRepository walletRepository)
        {
            _nodeLauncherFactory = nodeLauncherFactory;
            _transactionBuilder = transactionBuilder;
            _scriptBuilder = scriptBuilder;
            _walletRepository = walletRepository;
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

        private void SendMoney(object sender, EventArgs e)
        {
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                return;
            }

            var receiverValue = _viewModel.SendValue;
            var addr = _viewModel.SendAddress;
            var selectedTransaction = _viewModel.SelectedTransaction;
            if (selectedTransaction == null)
            {
                return;
            }

            if (receiverValue > selectedTransaction.Amount)
            {
                return;
            }

            double txFee = (DEFAULT_TX_SIZE / (double)1000) * Constants.DEFAULT_MIN_TX_FEE;
            var senderValue = selectedTransaction.Amount - receiverValue - txFee;
            var walletAddr = authenticatedWallet.Addresses.FirstOrDefault(a => a.Hash == selectedTransaction.Hash);
            if (walletAddr == null)
            {
                return;
            }

            BlockChainAddress bcAddr = null;
            try
            {
                bcAddr = BlockChainAddress.Deserialize(addr);
            }
            catch (Exception)
            {

            }

            if (bcAddr == null)
            {
                return;
            }

            var newKey = CreateNewAddress();
            var kh = new BigInteger(newKey.GetPublicKeyHashed());
            var script = _scriptBuilder.New()
                .AddToStack(walletAddr.Key.GetSignature())
                .AddToStack(walletAddr.Key.GetPublicKey())
                .Build();
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
            var tx = _transactionBuilder.NewNoneCoinbaseTransaction()
                .Spend(selectedTransaction.TxId.FromHexString(), (uint)selectedTransaction.Vout, script.Serialize())
                .AddOutput((long)receiverValue, receiverScript)
                .AddOutput((long)senderValue, senderSript)
                .Build();
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

                }
            });
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
            RefreshNbBlocks();
            RefreshMoney();
            RefreshBalance();
        }

        private void RefreshNbBlocks()
        {
            var blockChain = BlockChainStore.Instance().GetBlockChain();
            _viewModel.NbBlocks = blockChain.GetCurrentBlockHeight();
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
                lock(_lock)
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
                                foreach(var txUpdate in transactionsToUpdate)
                                {
                                    var tr = unspentTransactions.First(u => u.TxId == txUpdate.TxId && u.Vout == txUpdate.Vout);
                                    txUpdate.Amount = tr.Amount;
                                    txUpdate.DisplayName = string.Format("{0} : {1}", tr.Amount, tr.TxId);
                                }

                                foreach(var txRemove in transactionsToRemove)
                                {
                                    _viewModel.Transactions.Remove(txRemove);
                                }

                                var transactionsToAdd = unspentTransactions.Where(utxo => _viewModel.Transactions.All(tvm => tvm.TxId != utxo.TxId && tvm.Vout != utxo.Vout));
                                foreach (var transactionToAdd in transactionsToAdd)
                                {
                                    var txVm = new TransactionViewModel(transactionToAdd.TxId, transactionToAdd.Vout, transactionToAdd.Amount, transactionToAdd.Address);
                                    _viewModel.Transactions.Add(txVm);
                                }

                                _viewModel.Amount = unspentTransactions.Sum(t => t.Amount);
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
        }
    }
}
