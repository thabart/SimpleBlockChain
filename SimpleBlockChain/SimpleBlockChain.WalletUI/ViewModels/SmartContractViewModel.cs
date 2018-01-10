﻿using SimpleBlockChain.WalletUI.Commands;
using System;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class SmartContractViewModel : BaseViewModel
    {
        private string _smartContractAddress;
        private string _smartContractCallValue;
        private string _smartContract;
        private ICommand _callContractCommand;
        private ICommand _compileContractCommand;
        public event EventHandler CallContractEvt;
        public event EventHandler CompileContractEvt;

        public SmartContractViewModel()
        {
            _callContractCommand = new RelayCommand(p => CallSmartContractExecute(), p => CanExecuteCallSmartContract());
            _compileContractCommand = new RelayCommand(p => CompileContractExecute(), p => CanCompileContract());
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
    }
}
