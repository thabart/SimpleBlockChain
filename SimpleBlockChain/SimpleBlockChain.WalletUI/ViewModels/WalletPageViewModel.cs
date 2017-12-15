using SimpleBlockChain.Core;
using SimpleBlockChain.WalletUI.Commands;
using SimpleBlockChain.WalletUI.Events;
using System;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class WalletPageViewModel : BaseViewModel
    {
        private bool _isMainNetChecked;
        private bool _isTestNetChecked;
        private int _nbBlocks;
        private bool _isConnected;
        private ICommand _mainNetCommand;
        private ICommand _testNetCommand;
        private ICommand _refreshBlockChainCommand;

        public WalletPageViewModel()
        {
            _isMainNetChecked = true;
            _isTestNetChecked = false;
            _isConnected = false;
            _nbBlocks = 0;
            _mainNetCommand = new RelayCommand(p => ExecuteMainNet(), p => CanExecuteMainNet());
            _testNetCommand = new RelayCommand(p => ExecuteTestNet(), p => CanExecuteTestNet());
            _refreshBlockChainCommand = new RelayCommand(p => ExecuteRefreshBlockChain(), p => CanExecuteRefreshBlockChain());
        }

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
