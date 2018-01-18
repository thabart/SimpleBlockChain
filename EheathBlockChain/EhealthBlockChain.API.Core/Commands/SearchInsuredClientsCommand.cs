using System.Collections.Generic;

namespace EhealthBlockChain.API.Core.Commands
{
    public class SearchInsuredClientsCommand
    {
        public IEnumerable<string> FirstNames { get; set; }
        public IEnumerable<string> LastNames { get; set; }
        public IEnumerable<string> NationalRegistrationNumbers { get; set; }
    }
}
