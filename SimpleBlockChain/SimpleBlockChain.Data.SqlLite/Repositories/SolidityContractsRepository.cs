using Microsoft.EntityFrameworkCore;
using SimpleBlockChain.Core.Aggregates;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Data.Sqlite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlockChain.Data.Sqlite.Repositories
{
    public class SolidityContractsRepository : ISolidityContractsRepository
    {
        private readonly CurrentDbContext _currentDbContext;

        public SolidityContractsRepository(CurrentDbContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public async Task<IEnumerable<SolidityContractAggregate>> GetAll()
        {
            var solidityContracts = await _currentDbContext.SolidityContracts.Include(s => s.Filters).ToListAsync().ConfigureAwait(false);
            var result = new List<SolidityContractAggregate>();
            foreach(var solidityContract in solidityContracts)
            {
                result.Add(new SolidityContractAggregate
                {
                    Address = solidityContract.Address,
                    Code = solidityContract.Code,
                    Abi = solidityContract.Abi,
                    Filters = solidityContract.Filters == null ? null : solidityContract.Filters.Select(f => f.Id)
                });
            }

            return result;
        }

        public async Task<SolidityContractAggregate> Get(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentNullException(nameof(address));
            }

            var result = await _currentDbContext.SolidityContracts.Include(s => s.Filters).FirstOrDefaultAsync(c => c.Address == address);
            if (result == null)
            {
                return null;
            }

            return new SolidityContractAggregate
            {
                Address = result.Address,
                Code = result.Code,
                Abi = result.Abi,
                Filters = result.Filters == null ? null : result.Filters.Select(f => f.Id)
            };
        }

        public async Task<bool> Insert(SolidityContractAggregate contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            var exists = await _currentDbContext.SolidityContracts.AnyAsync(w => w.Address == contract.Address).ConfigureAwait(false);
            if (exists)
            {
                return false;
            }

            var record = new SolidityContract
            {
                Address = contract.Address,
                Code = contract.Code,
                Abi = contract.Abi
            };
            _currentDbContext.SolidityContracts.Add(record);
            await _currentDbContext.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }
    }
}
