using SimpleBlockChain.WalletUI.Commands;
using System;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class SmartContractViewModel : BaseViewModel
    {
        private string _smartContractAddress;
        private string _smartContractCallValue;
        private ICommand _callContractCommand;
        public event EventHandler CallContractEvt;

        public SmartContractViewModel()
        {
            _callContractCommand = new RelayCommand(p => CallSmartContractExecute(), p => CanExecuteCallSmartContract());
        }

        public string SmartContractAddress
        {
            get { return _smartContractAddress; }
            set
            {
                if (value != _smartContractAddress)
                {
                    _smartContractAddress = value;
                    NotifyPropertyChanged(nameof(SmartContractAddress));
                }
            }
        }

        public string SmartContractCallValue
        {
            get { return _smartContractCallValue; }
            set
            {
                if (value != _smartContractCallValue)
                {
                    _smartContractCallValue = value;
                    NotifyPropertyChanged(nameof(SmartContractCallValue));
                }
            }
        }

        public ICommand CallContractCommand
        {
            get
            {
                return _callContractCommand;
            }
        }

        private void CallSmartContractExecute()
        {
            if (CallContractEvt != null)
            {
                CallContractEvt(this, EventArgs.Empty);
            }
        }

        private bool CanExecuteCallSmartContract()
        {
            return true;
        }
    }
}
