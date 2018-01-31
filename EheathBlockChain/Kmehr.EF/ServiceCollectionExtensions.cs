using Kmehr.Core.Repositories;
using Kmehr.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Kmehr.EF
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKmehrInMemory(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            RegisterServices(serviceCollection);
            serviceCollection.AddEntityFrameworkInMemoryDatabase()
                             .AddDbContext<KmehrDbContext>(options => options.UseInMemoryDatabase().ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

            return serviceCollection;
        }

        private static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IHealthCarePartyTypeRepository, HealthCarePartyTypeRepository>();
        }
    }
}
