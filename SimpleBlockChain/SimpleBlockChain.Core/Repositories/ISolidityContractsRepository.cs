using SimpleBlockChain.Core.Aggregates;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleBlockChain.Core.Repositories
{
    public interface ISolidityContractsRepository
    {
        Task<IEnumerable<SolidityContractAggregate>> GetAll();
        Task<SolidityContractAggregate> Get(string address);
        Task<bool> Insert(SolidityContractAggregate contract);
    }
}
