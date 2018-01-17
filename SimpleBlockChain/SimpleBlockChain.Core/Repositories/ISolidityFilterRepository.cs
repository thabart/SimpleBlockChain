using System.Threading.Tasks;

namespace SimpleBlockChain.Core.Repositories
{
    public interface ISolidityFilterRepository
    {
        Task<bool> Add(string scAddr, string filterId);
    }
}
