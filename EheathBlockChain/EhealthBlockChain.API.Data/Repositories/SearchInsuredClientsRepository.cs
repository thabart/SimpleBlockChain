using System.Threading.Tasks;
using EhealthBlockChain.API.Core.Commands;
using EhealthBlockChain.API.Core.Repositories;
using EhealthBlockChain.API.Core.Responses;
using System;

namespace EhealthBlockChain.API.Data.Repositories
{
    internal sealed class InsuredClientsRepository : IInsuredClientsRepository
    {
        private readonly CurrentDbContext _currentDbContext;

        public InsuredClientsRepository(CurrentDbContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public Task<SearchInsuredClientsResponse> Search(SearchInsuredClientsCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            throw new System.NotImplementedException();
        }
    }
}
