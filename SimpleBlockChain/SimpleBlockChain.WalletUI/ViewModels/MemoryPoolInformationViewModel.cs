using System.Collections.ObjectModel;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class RawMemPoolViewModel
    {
        public string TxId { get; set; }
        public long Time { get; set; }
        public long Fee { get; set; }
    }

    public class MemoryPoolInformationViewModel
    {
        public MemoryPoolInformationViewModel()
        {
            Raws = new ObservableCollection<RawMemPoolViewModel>();
        }

        public ObservableCollection<RawMemPoolViewModel> Raws { get; set; }
        public RawMemPoolViewModel SelectedRaw { get; set; }

        public void Reset()
        {
            Raws = new ObservableCollection<RawMemPoolViewModel>();
            SelectedRaw = null;
        }
    }
}
