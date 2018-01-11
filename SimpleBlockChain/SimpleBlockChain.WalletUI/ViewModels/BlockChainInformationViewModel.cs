using SimpleBlockChain.WalletUI.Commands;
using SimpleBlockChain.WalletUI.Events;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class BlockViewModel
    {
        public string Hash { get; set; }
        public string PreviousHash { get; set; }
        public long Fees { get; set; }
    }

    public class BlockChainInformationViewModel
    {
        private ICommand _refreshCommand;
        private ICommand _selectBlockCommand;

        public BlockChainInformationViewModel()
        {
            _refreshCommand = new RelayCommand(p => ExecuteRefresh(), p => CanExecuteRefresh());
            _selectBlockCommand = new RelayCommand(p => ExecuteSelectBlock(), p => CanSelectBlock());
            Blocks = new ObservableCollection<BlockViewModel>();
        }
        
        public event EventHandler RefreshEvt;
        public event EventHandler<BlockEventArgs> SelectBlockEvt;
        public ObservableCollection<BlockViewModel> Blocks { get; private set; }
        public BlockViewModel SelectedBlock { get; set; }

        public void Reset()
        {
            Blocks = new ObservableCollection<BlockViewModel>();
        }

        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand;
            }
        }

        public ICommand SelectBlockCommand
        {
            get
            {
                return _selectBlockCommand;
            }
        }

        private void ExecuteRefresh()
        {
            if (RefreshEvt != null)
            {
                RefreshEvt(this, EventArgs.Empty);
            }
        }

        private bool CanExecuteRefresh()
        {
            return true;
        }

        private void ExecuteSelectBlock()
        {
            if (SelectedBlock == null)
            {
                return;
            }

            if (SelectBlockEvt != null)
            {
                SelectBlockEvt(this, new BlockEventArgs(SelectedBlock.Hash));
            }
        }

        private bool CanSelectBlock()
        {
            return true;
        }
    }
}
