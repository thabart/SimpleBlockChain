using EhealthBlockChain.API.Core.Aggregates;
using System.Collections.Generic;

namespace EhealthBlockChain.API.Core.Responses
{
    public sealed class SearchInsuredClientsResponse
    {
        public IEnumerable<InsuredClientAggregate> InsuredClients { get; set; }
    }
}
