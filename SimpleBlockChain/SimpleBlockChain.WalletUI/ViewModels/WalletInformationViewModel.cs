using MahApps.Metro.Controls.Dialogs;
using SimpleBlockChain.WalletUI.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class TransactionViewModel
    {
        public TransactionViewModel(string txId, int vout, double amount, string hash, int confirmation)
        {
            TxId = txId;
            Vout = vout;
            Amount = amount;
            Hash = hash;
            if (confirmation == 0)
            {
                DisplayName = string.Format("Unconfirmed TRANSACTION {0}: {1}", amount, txId);
            }
            else
            {
                DisplayName = string.Format("{0} : {1}", amount, txId);
            }
        }

        public string DisplayName { get; set; }
        public string TxId { get; set; }
        public double Amount { get; set; }
        public int Vout { get; set; }
        public string Hash { get; set; }

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

    public class WalletInformationViewModel : BaseViewModel
    {
        private double _sendValue;
        private string _sendAddress;
        private ICommand _sendMoneyCommand;
        private int _amount;
        public int _balance;

        public WalletInformationViewModel()
        {
            _sendMoneyCommand = new RelayCommand(p => SendMoneyExecute(), p => CanSendMoney());
            Transactions = new ObservableCollection<TransactionViewModel>();
            _amount = 0;
            _balance = 0;
        }

        public ICommand SendMoney { get; private set; }
        public event EventHandler SendMoneyEvt;
        public ObservableCollection<TransactionViewModel> Transactions { get; private set; }
        public TransactionViewModel SelectedTransaction { get; set; }

        public void Reset()
        {
            Amount = 0;
            Balance = 0;
            Transactions = new ObservableCollection<TransactionViewModel>();
        }

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

        public ICommand SendMoneyCommand
        {
            get
            {
                return _sendMoneyCommand;
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
    }
}
