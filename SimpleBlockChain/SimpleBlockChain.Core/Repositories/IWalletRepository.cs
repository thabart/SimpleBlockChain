using SimpleBlockChain.Core.Aggregates;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleBlockChain.Core.Repositories
{
    public interface IWalletRepository
    {
        Task<bool> Add(WalletAggregate wallet);
        Task<IEnumerable<WalletAggregate>> GetAll();
        Task<WalletAggregate> Get(string name);
    }
}
