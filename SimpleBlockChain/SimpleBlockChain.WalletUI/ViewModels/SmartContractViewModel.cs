using SimpleBlockChain.Core.Aggregates;
using SimpleBlockChain.WalletUI.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class FunctionDefinitionViewModel
    {
        public FunctionDefinitionViewModel()
        {
            Parameters = new ObservableCollection<ParameterDefinitionViewModel>();
        }

        public string FunctionName { get; set; }
        public ObservableCollection<ParameterDefinitionViewModel> Parameters { get; set; }
        public SolidityContractAggregateFunction FunctionAgg { get; set; }
    }

    public class ParameterDefinitionViewModel : BaseViewModel
    {
        private string _value;

        public string Name { get; set; }
        public string Type { get; set; }
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    NotifyPropertyChanged(nameof(Value));
                }
            }
        }
    }

    public class FilterViewModel
    {
        public string Id { get; set; }
    }

    public class SolidityContractViewModel
    {
        public SolidityContractViewModel()
        {
            Functions = new ObservableCollection<FunctionDefinitionViewModel>();
            Filters = new ObservableCollection<FilterViewModel>();
        }

        public string Address { get; set; }
        public ObservableCollection<FunctionDefinitionViewModel> Functions { get; set; }
        public ObservableCollection<FilterViewModel> Filters { get; set; }
        public SolidityContractAggregate SolidityContractAgg { get; set; }
    }

    public class SmartContractViewModel : BaseViewModel
    {
        private string _newSmartContractAddress;
        private string _smartContractCallValue;
        private string _smartContract;
        private string _transactionId;
        private string _generatedCallValue;
        private ICommand _callContractCommand;
        private ICommand _compileContractCommand;
        private ICommand _publishContractCommand;
        private ICommand _publishTransactionCallCommand;
        private ICommand _persistSmartContractCommand;
        private ICommand _refreshContractsCommand;
        private ICommand _listenSmartContractCommand;
        private ICommand _getLastLogsCommand;
        public event EventHandler CallContractEvt;
        public event EventHandler CompileContractEvt;
        public event EventHandler PublishContractEvt;
        public event EventHandler PublishTransactionCallEvt;
        public event EventHandler PersistSmartContractEvt;
        public event EventHandler RefreshContractsEvt;
        public event EventHandler ListenSmartContractEvt;
        public event EventHandler GetLastLogsEvt;
        private ObservableCollection<SolidityContractViewModel> _smartContracts;
        private SolidityContractViewModel _selectedSolidityContract;
        private FunctionDefinitionViewModel _selectedFunctionDefinition;
        private FilterViewModel _selectedFilter;

        public SmartContractViewModel()
        {
            _callContractCommand = new RelayCommand(p => CallSmartContractExecute(), p => CanExecuteCallSmartContract());
            _compileContractCommand = new RelayCommand(p => CompileContractExecute(), p => CanCompileContract());
            _publishContractCommand = new RelayCommand(p => PublishContractExecute(), p => CanPublishContract());
            _publishTransactionCallCommand = new RelayCommand(p => PublishTransactionCallExecute(), p => CanPublishTransactionCallExecute());
            _persistSmartContractCommand = new RelayCommand(p => PersistSmartContractExecute(), p => CanExecutePersistSmartContract());
            _refreshContractsCommand = new RelayCommand(p => RefreshContractsExecute(), p => CanExecuteRefreshContracts());
            _listenSmartContractCommand = new RelayCommand(p => ListenSmartContractExecute(), p => CanExecuteListenSmartContract());
            _getLastLogsCommand = new RelayCommand(p => GetLastLogsExecute(), p => CanExecuteGetLastLogs());
            _smartContracts = new ObservableCollection<SolidityContractViewModel>();
        }

        #region Properties

        public FilterViewModel SelectedFilter
        {
            get
            {
                return _selectedFilter;
            }
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    NotifyPropertyChanged(nameof(SelectedFilter));
                }
            }
        }

        public FunctionDefinitionViewModel SelectedFunctionDefinition
        {
            get
            {
                return _selectedFunctionDefinition;
            }
            set
            {
                if (_selectedFunctionDefinition != value)
                {
                    _selectedFunctionDefinition = value;
                    NotifyPropertyChanged(nameof(SelectedFunctionDefinition));
                }
            }
        }

        public SolidityContractViewModel SelectedSolidityContract
        {
            get
            {
                return _selectedSolidityContract;
            }
            set
            {
                if (_selectedSolidityContract != value)
                {
                    _selectedSolidityContract = value;
                    NotifyPropertyChanged(nameof(SelectedSolidityContract));
                }
            }
        }

        public ObservableCollection<SolidityContractViewModel> SmartContracts
        {
            get
            {
                return _smartContracts;
            }
        }

        public string NewSmartContractAddress
        {
            get
            {
                return _newSmartContractAddress;
            }
            set
            {
                if (_newSmartContractAddress != value)
                {
                    _newSmartContractAddress = value;
                    NotifyPropertyChanged(nameof(NewSmartContractAddress));
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

        public string GeneratedCallValue
        {
            get
            {
                return _generatedCallValue;
            }
            set
            {
                if (_generatedCallValue != value)
                {
                    _generatedCallValue = value;
                    NotifyPropertyChanged(nameof(GeneratedCallValue));
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

        public ICommand PublishTransactionCallCommand
        {
            get
            {
                return _publishTransactionCallCommand;
            }
        }

        public ICommand PersistSmartContractCommand
        {
            get
            {
                return _persistSmartContractCommand;
            }
        }

        public ICommand RefreshContractsCommand
        {
            get
            {
                return _refreshContractsCommand;
            }
        }

        public ICommand ListenSmartContractCommand
        {
            get
            {
                return _listenSmartContractCommand;
            }
        }

        public ICommand GetLastLogsCommand
        {
            get
            {
                return _getLastLogsCommand;
            }
        }

        #endregion

        #region Private methods

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

        private void PersistSmartContractExecute()
        {
            if (PersistSmartContractEvt != null)
            {
                PersistSmartContractEvt(this, EventArgs.Empty);
            }
        }

        private bool CanExecutePersistSmartContract()
        {
            return true;
        }

        private void RefreshContractsExecute()
        {
            if (RefreshContractsEvt != null)
            {
                RefreshContractsEvt(this, EventArgs.Empty);
            }
        }

        private bool CanExecuteRefreshContracts()
        {
            return true;
        }

        private void ListenSmartContractExecute()
        {
            if (ListenSmartContractEvt != null)
            {
                ListenSmartContractEvt(this, EventArgs.Empty);
            }
        }

        private bool CanExecuteListenSmartContract()
        {
            return true;
        }

        private void GetLastLogsExecute()
        {
            if (GetLastLogsEvt != null)
            {
                GetLastLogsEvt(this, EventArgs.Empty);
            }
        }

        private bool CanExecuteGetLastLogs()
        {
            return true;
        }

        #endregion
    }
}
