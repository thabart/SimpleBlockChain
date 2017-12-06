using SimpleBlockChain.Core.Aggregates;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace SimpleBlockChain.Core.Repositories
{
    public interface IWalletRepository
    {
        Task<bool> Add(WalletAggregate wallet, SecureString password);
        Task<IEnumerable<string>> GetAll();
        Task<WalletAggregate> Get(string name, SecureString password);
    }
}
