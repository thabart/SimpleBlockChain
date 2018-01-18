using EhealthBlockChain.API.Core.Aggregates;
using EhealthBlockChain.API.Data.Models;
using System;

namespace EhealthBlockChain.API.Data.Extensions
{
    internal static class MappingExtension
    {
        public static InsuredClientAggregate ToDto(this InsuredClient insuredClient)
        {
            if (insuredClient == null)
            {
                throw new ArgumentNullException(nameof(insuredClient));
            }

            return new InsuredClientAggregate
            {
                FirstName = insuredClient.FirstName,
                LastName = insuredClient.LastName,
                NationalRegistrationNumber = insuredClient.NationalRegistrationNumber
            };
        }
    }
}
