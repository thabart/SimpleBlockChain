
using Kmehr.Core.Models;
using System.Threading.Tasks;

namespace Kmehr.Core.Repositories
{
    public interface IHealthCarePartyTypeRepository
    {
        Task<HealthCarePartyTypeAggregate> Get(string code);
    }
}
