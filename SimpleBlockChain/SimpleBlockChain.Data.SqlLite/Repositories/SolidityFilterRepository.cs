using System.Threading.Tasks;
using SimpleBlockChain.Core.Repositories;
using System;
using Microsoft.EntityFrameworkCore;
using SimpleBlockChain.Data.Sqlite.Models;

namespace SimpleBlockChain.Data.Sqlite.Repositories
{
    public class SolidityFilterRepository : ISolidityFilterRepository
    {
        private readonly CurrentDbContext _currentDbContext;

        public SolidityFilterRepository(CurrentDbContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public async Task<bool> Add(string scAddr, string filterId)
        {
            if (string.IsNullOrWhiteSpace(scAddr))
            {
                throw new ArgumentNullException(nameof(scAddr));
            }

            if (string.IsNullOrWhiteSpace(filterId))
            {
                throw new ArgumentNullException(nameof(filterId));
            }

            var exists = await _currentDbContext.SolidityContracts.AnyAsync(w => w.Address == scAddr).ConfigureAwait(false);
            if (!exists)
            {
                return false;
            }

            _currentDbContext.SolidityFilters.Add(new SolidityFilter
            {
                Id = filterId,
                SmartContractAddress = scAddr
            });
            await _currentDbContext.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }
    }
}
