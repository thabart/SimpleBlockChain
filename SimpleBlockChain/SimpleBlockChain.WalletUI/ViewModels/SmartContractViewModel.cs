﻿using SimpleBlockChain.WalletUI.Commands;
using System;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class SmartContractViewModel : BaseViewModel
    {
        private string _getSmartContractAddressResult;
        private string _smartContractAddress;
        private string _smartContractCallValue;
        private string _smartContract;
        private string _transactionAddress;
        private string _transactionId;
        private ICommand _callContractCommand;
        private ICommand _compileContractCommand;
        private ICommand _publishContractCommand;
        private ICommand _getSmartContractCommand;
        private ICommand _publishTransactionCallCommand;
        public event EventHandler CallContractEvt;
        public event EventHandler CompileContractEvt;
        public event EventHandler PublishContractEvt;
        public event EventHandler GetSmartContractEvt;
        public event EventHandler PublishTransactionCallEvt;

        public SmartContractViewModel()
        {
            _callContractCommand = new RelayCommand(p => CallSmartContractExecute(), p => CanExecuteCallSmartContract());
            _compileContractCommand = new RelayCommand(p => CompileContractExecute(), p => CanCompileContract());
            _publishContractCommand = new RelayCommand(p => PublishContractExecute(), p => CanPublishContract());
            _getSmartContractCommand = new RelayCommand(p => GetSmartContractExecute(), p => CanExecuteGetSmartContract());
            _publishTransactionCallCommand = new RelayCommand(p => PublishTransactionCallExecute(), p => CanPublishTransactionCallExecute());
        }

        public string TransactionAddress
        {
            get { return _transactionAddress; }
            set
            {
                if (_transactionAddress != value)
                {
                    _transactionAddress = value;
                    NotifyPropertyChanged(nameof(TransactionAddress));
                }
            }
        }

        public string GetSmartContractAddressResult
        {
            get
            {
                return _getSmartContractAddressResult;
            }
            set
            {
                if (_getSmartContractAddressResult != value)
                {
                    _getSmartContractAddressResult = value;
                    NotifyPropertyChanged(nameof(GetSmartContractAddressResult));
                }
            }
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

        public string SmartContract
        {
            get
            {
                return _smartContract;
            }
            set
            {
                if (value != _smartContract)
                {
                    _smartContract = value;
                    NotifyPropertyChanged(nameof(SmartContract));
                }
            }
        }

        public string TransactionId
        {
            get
            {
                return _transactionId;
            }
            set
            {
                if (_transactionId != value)
                {
                    _transactionId = value;
                    NotifyPropertyChanged(nameof(TransactionId));
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

        public ICommand CompileContractCommand
        {
            get
            {
                return _compileContractCommand;
            }
        }

        public ICommand PublishContractCommand
        {
            get
            {
                return _publishContractCommand;
            }
        }

        public ICommand GetSmartContractCommand
        {
            get
            {
                return _getSmartContractCommand;
            }
        }

        public ICommand PublishTransactionCallCommand
        {
            get
            {
                return _publishTransactionCallCommand;
            }
        }

        private void PublishTransactionCallExecute()
        {
            if (PublishTransactionCallEvt != null)
            {
                PublishTransactionCallEvt(this, EventArgs.Empty);
            }
        }

        private bool CanPublishTransactionCallExecute()
        {
            return true;
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

        private void CompileContractExecute()
        {
            if (CompileContractEvt != null)
            {
                CompileContractEvt(this, EventArgs.Empty);
            }
        }

        private bool CanCompileContract()
        {
            return true;
        }

        private void PublishContractExecute()
        {
            if (PublishContractEvt != null)
            {
                PublishContractEvt(this, EventArgs.Empty);
            }
        }

        private bool CanPublishContract()
        {
            return true;
        }

        private void GetSmartContractExecute()
        {
            if (GetSmartContractEvt != null)
            {
                GetSmartContractEvt(this, EventArgs.Empty);
            }
        }

        private bool CanExecuteGetSmartContract()
        {
            return true;
        }
    }
}
