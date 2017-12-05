using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Aggregates;

namespace SimpleBlockChain.Data.Sqlite.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        public Task<bool> Add(WalletAggregate wallet)
        {
            if (wallet == null)
            {
                throw new ArgumentNullException(nameof(wallet));
            }

            return Task.FromResult(true);
        }

        public Task<WalletAggregate> Get(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WalletAggregate>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
