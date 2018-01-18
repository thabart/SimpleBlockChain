using System.Threading.Tasks;
using EhealthBlockChain.API.Core.Commands;
using EhealthBlockChain.API.Core.Repositories;
using EhealthBlockChain.API.Core.Responses;
using System;
using System.Linq;
using EhealthBlockChain.API.Data.Models;
using Microsoft.EntityFrameworkCore;
using EhealthBlockChain.API.Data.Extensions;

namespace EhealthBlockChain.API.Data.Repositories
{
    internal sealed class InsuredClientsRepository : IInsuredClientsRepository
    {
        private readonly CurrentDbContext _currentDbContext;

        public InsuredClientsRepository(CurrentDbContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public async Task<bool> Add()
        {
            using (var transaction = await _currentDbContext.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                return true;
            }
        }

        public async Task<SearchInsuredClientsResponse> Search(SearchInsuredClientsCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            IQueryable<InsuredClient> insuredClients = _currentDbContext.InsuredClients;
            if (command.FirstNames != null)
            {
                insuredClients = insuredClients.Where(i => command.FirstNames.Contains(i.FirstName));
            }

            if (command.LastNames != null)
            {
                insuredClients = insuredClients.Where(i => command.LastNames.Contains(i.LastName));
            }

            if (command.NationalRegistrationNumbers != null)
            {
                insuredClients = insuredClients.Where(i => command.NationalRegistrationNumbers.Contains(i.NationalRegistrationNumber));
            }

            var result = await insuredClients.ToListAsync().ConfigureAwait(false);
            if (result != null && result.Any())
            {
                return new SearchInsuredClientsResponse
                {
                    InsuredClients = result.Select(r => r.ToDto())
                };
            }

            return null;
        }
    }
}
