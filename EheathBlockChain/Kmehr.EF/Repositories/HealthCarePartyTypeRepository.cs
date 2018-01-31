using Kmehr.Core.Models;
using Kmehr.Core.Repositories;
using Kmehr.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmehr.EF.Repositories
{
    internal sealed class HealthCarePartyTypeRepository : IHealthCarePartyTypeRepository
    {
        private readonly IServiceProvider _serviceProvider;

        public HealthCarePartyTypeRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<HealthCarePartyTypeAggregate> Get(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<KmehrDbContext>())
                {
                    var record = await context.HealthCarePartyTypes.Include(h => h.Translations).FirstOrDefaultAsync(h => h.Code == code).ConfigureAwait(false);
                    if (record == null)
                    {
                        return null;
                    }

                    return GetModel(record);
                }
            }
        }

        private static HealthCarePartyTypeAggregate GetModel(HealthCarePartyType healthCarePartyType)
        {
            return new HealthCarePartyTypeAggregate
            {
                Code = healthCarePartyType.Code,
                Descriptions = healthCarePartyType.Translations == null ? new List<HealthCarePartyTypeAggregateDescription>() : healthCarePartyType.Translations.Select(t =>
                    new HealthCarePartyTypeAggregateDescription
                    {
                        Language = t.LanguageId,
                        Value = t.Value
                    }
                )
            };
        }
    }
}
