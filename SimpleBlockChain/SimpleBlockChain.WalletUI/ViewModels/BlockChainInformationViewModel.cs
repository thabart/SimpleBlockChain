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
        private ICommand _previousPageCommand;
        private ICommand _nextPageCommand;
        private ICommand _selectBlockCommand;

        public BlockChainInformationViewModel()
        {
            _previousPageCommand = new RelayCommand(p => ExecutePreviousPage(), p => CanExecutePreviousPage());
            _nextPageCommand = new RelayCommand(p => ExecuteNextPage(), p => CanExecuteNextPage());
            _selectBlockCommand = new RelayCommand(p => ExecuteSelectBlock(), p => CanSelectBlock());
            Blocks = new ObservableCollection<BlockViewModel>();
        }

        public event EventHandler PreviousPageEvt;
        public event EventHandler NextPageEvt;
        public event EventHandler<BlockEventArgs> SelectBlockEvt;
        public ObservableCollection<BlockViewModel> Blocks { get; private set; }
        public BlockViewModel SelectedBlock { get; set; }

        public ICommand PreviousPageCommand
        {
            get
            {
                return _previousPageCommand;
            }
        }

        public ICommand NextPageCommand
        {
            get
            {
                return _nextPageCommand;
            }
        }

        public ICommand SelectBlockCommand
        {
            get
            {
                return _selectBlockCommand;
            }
        }

        private void ExecutePreviousPage()
        {
            if (PreviousPageEvt != null)
            {
                PreviousPageEvt(this, EventArgs.Empty);
            }
        }

        private bool CanExecutePreviousPage()
        {
            return true;
        }

        private void ExecuteNextPage()
        {
            if (NextPageEvt != null)
            {
                NextPageEvt(this, EventArgs.Empty);
            }
        }

        private bool CanExecuteNextPage()
        {
            return true;
        }

        private void ExecuteSelectBlock()
        {
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
