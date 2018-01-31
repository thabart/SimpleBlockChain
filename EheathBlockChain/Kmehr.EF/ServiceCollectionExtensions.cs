using Kmehr.Core.Repositories;
using Kmehr.EF.Extensions;
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

        public static IServiceProvider EnsureSeedData(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }


            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var currentDbContext = serviceScope.ServiceProvider.GetService<KmehrDbContext>();
                currentDbContext.EnsureSeedData();
            }

            return serviceProvider;
        }

        private static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IHealthCarePartyTypeRepository, HealthCarePartyTypeRepository>();
        }
    }
}
