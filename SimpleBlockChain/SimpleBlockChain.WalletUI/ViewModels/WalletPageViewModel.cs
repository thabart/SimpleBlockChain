using SimpleBlockChain.WalletUI.Commands;
using System;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class WalletPageViewModel : BaseViewModel
    {
        private double _sendValue;
        public string _sendAddress;

        public WalletPageViewModel()
        {
            SendMoney = new RelayCommand(p => SendMoneyExecute(), p => CanSendMoney());
        }

        public ICommand SendMoney { get; private set; }
        public event EventHandler SendMoneyEvt;

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
    }
}
