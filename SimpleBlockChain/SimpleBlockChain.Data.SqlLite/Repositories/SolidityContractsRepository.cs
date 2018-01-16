using Microsoft.EntityFrameworkCore;
using SimpleBlockChain.Core.Aggregates;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Data.Sqlite.Models;
using System;
using System.Collections.Generic;
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
            var solidityContracts = await _currentDbContext.SolidityContracts.ToListAsync().ConfigureAwait(false);
            var result = new List<SolidityContractAggregate>();
            foreach(var solidityContract in solidityContracts)
            {
                result.Add(new SolidityContractAggregate
                {
                    Address = solidityContract.Address,
                    Code = solidityContract.Code,
                    Abi = solidityContract.Abi
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

            var result = await _currentDbContext.SolidityContracts.FirstOrDefaultAsync(c => c.Address == address);
            if (result == null)
            {
                return null;
            }

            return new SolidityContractAggregate
            {
                Address = result.Address,
                Code = result.Code,
                Abi = result.Abi
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
