using EhealthBlockChain.API.Core.Commands;
using EhealthBlockChain.API.Core.Responses;
using System.Threading.Tasks;

namespace EhealthBlockChain.API.Core.Repositories
{
    public interface IInsuredClientsRepository
    {
        Task<bool> Add();
        Task<SearchInsuredClientsResponse> Search(SearchInsuredClientsCommand command);
    }
}
