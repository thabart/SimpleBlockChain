using SimpleBlockChain.Core;
using SimpleBlockChain.WalletUI.Commands;
using SimpleBlockChain.WalletUI.Events;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class TransactionViewModel
    {
        public TransactionViewModel(string txId, int vout, int amount, string hash)
        {
            TxId = txId;
            Vout = vout;
            Amount = amount;
            Hash = hash;
            DisplayName = string.Format("{0} : {1}", amount, txId);
        }
        
        public string DisplayName { get; set; }
        public string TxId { get; private set; }
        public int Amount { get; set; }
        public int Vout { get; private set; }
        public string Hash { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var trVm = obj as TransactionViewModel;
            if (trVm == null)
            {
                return false;
            }

            return Vout == trVm.Vout && TxId == trVm.TxId;
        }
    }

    public class WalletPageViewModel : BaseViewModel
    {
        private double _sendValue;
        private string _sendAddress;
        private bool _isMainNetChecked;
        private bool _isTestNetChecked;
        private int _nbBlocks;
        private bool _isConnected;
        private int _amount;
        public int _balance;
        private ICommand _mainNetCommand;
        private ICommand _testNetCommand;
        private ICommand _refreshBlockChainCommand;
        private ICommand _sendMoneyCommand;

        public WalletPageViewModel()
        {
            _isMainNetChecked = true;
            _isTestNetChecked = false;
            _isConnected = false;
            _nbBlocks = 0;
            _amount = 0;
            Transactions = new ObservableCollection<TransactionViewModel>();
            _sendMoneyCommand = new RelayCommand(p => SendMoneyExecute(), p => CanSendMoney());
            _mainNetCommand = new RelayCommand(p => ExecuteMainNet(), p => CanExecuteMainNet());
            _testNetCommand = new RelayCommand(p => ExecuteTestNet(), p => CanExecuteTestNet());
            _refreshBlockChainCommand = new RelayCommand(p => ExecuteRefreshBlockChain(), p => CanExecuteRefreshBlockChain());
        }

        public ICommand SendMoney { get; private set; }
        public event EventHandler SendMoneyEvt;
        public event EventHandler<NetworkEventHandler> NetworkSwitchEvt;
        public event EventHandler RefreshBlockChainEvt;

        public ICommand MainNetCommand
        {
            get { return _mainNetCommand; }
        }

        public ICommand TestNetCommand
        {
            get { return _testNetCommand; }
        }

        public ICommand RefreshBlockChainCommand
        {
            get
            {
                return _refreshBlockChainCommand;
            }
        }

        public ICommand SendMoneyCommand
        {
            get
            {
                return _sendMoneyCommand;
            }
        }

        public ObservableCollection<TransactionViewModel> Transactions { get; private set; }

        public TransactionViewModel SelectedTransaction { get; set; }

        public int Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    NotifyPropertyChanged(nameof(Amount));
                }
            }
        }

        public int Balance
        {
            get
            {
                return _balance;
            }
            set
            {
                if (_balance != value)
                {
                    _balance = value;
                    NotifyPropertyChanged(nameof(Balance));
                }
            }
        }

        public bool IsMainNetChecked
        {
            get
            {
                return _isMainNetChecked;
            }
            set
            {
                if (value != _isMainNetChecked)
                {
                    _isMainNetChecked = value;
                    NotifyPropertyChanged(nameof(IsMainNetChecked));
                }
            }
        }

        public bool IsTestNetChecked
        {
            get
            {
                return _isTestNetChecked;
            }
            set
            {
                if (value != _isTestNetChecked)
                {
                    _isTestNetChecked = value;
                    NotifyPropertyChanged(nameof(IsTestNetChecked));
                }
            }
        }

        public int NbBlocks
        {
            get
            {
                return _nbBlocks;
            }
            set
            {
                if (value != _nbBlocks)
                {
                    _nbBlocks = value;
                    NotifyPropertyChanged(nameof(NbBlocks));
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    NotifyPropertyChanged(nameof(IsConnected));
                }
            }
        }

        public double SendValue
        {
            get
            {
                return _sendValue;
            }
            set
            {
                if (value != _sendValue)
                {
                    _sendValue = value;
                    NotifyPropertyChanged(nameof(SendValue));
                }
            }
        }

        public string SendAddress
        {
            get
            {
                return _sendAddress;
            }
            set
            {
                if (_sendAddress != value)
                {
                    _sendAddress = value;
                    NotifyPropertyChanged(nameof(SendAddress));
                }
            }
        }

        private bool CanSendMoney()
        {
            return true;
        }

        private void SendMoneyExecute()
        {
            if (SendMoneyEvt != null)
            {
                SendMoneyEvt(this, EventArgs.Empty);
            }
        }

        public void ExecuteMainNet()
        {
            IsMainNetChecked = true;
            IsTestNetChecked = false;
            if (NetworkSwitchEvt != null)
            {
                NetworkSwitchEvt(this, new NetworkEventHandler(Networks.MainNet));
            }
        }

        private bool CanExecuteMainNet()
        {
            return true;
        }

        public void ExecuteTestNet()
        {
            IsTestNetChecked = true;
            IsMainNetChecked = false;
            if (NetworkSwitchEvt != null)
            {
                NetworkSwitchEvt(this, new NetworkEventHandler(Networks.TestNet));
            }
        }

        private bool CanExecuteTestNet()
        {
            return true;
        }

        private void ExecuteRefreshBlockChain()
        {
            if (RefreshBlockChainEvt != null)
            {
                RefreshBlockChainEvt(this, EventArgs.Empty);
            }
        }

        private bool CanExecuteRefreshBlockChain()
        {
            return true;
        }
    }
}
