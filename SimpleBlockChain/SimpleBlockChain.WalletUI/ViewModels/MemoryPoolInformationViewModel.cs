using SimpleBlockChain.WalletUI.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class RawMemPoolViewModel
    {
        public string TxId { get; set; }
        public long Time { get; set; }
        public long Fee { get; set; }
        public int AncestorCount { get; set; }
        public int DescendantCount { get; set; }
    }

    public class MemoryPoolInformationViewModel
    {
        private ICommand _refreshCommand;

        public MemoryPoolInformationViewModel()
        {
            Raws = new ObservableCollection<RawMemPoolViewModel>();
            _refreshCommand = new RelayCommand(p => ExecuteRefresh(), p => CanExecuteRefresh());
        }

        public event EventHandler RefreshEvt;
        public ObservableCollection<RawMemPoolViewModel> Raws { get; set; }
        public RawMemPoolViewModel SelectedRaw { get; set; }

        public void Reset()
        {
            Raws = new ObservableCollection<RawMemPoolViewModel>();
            SelectedRaw = null;
        }

        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand;
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
    }
}
