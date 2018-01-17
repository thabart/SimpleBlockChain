using EhealthBlockChain.API.Core.Repositories;
using EhealthBlockChain.API.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EhealthBlockChain.API.Data
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInMemory(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            RegisterServices(serviceCollection);
            serviceCollection.AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<CurrentDbContext>(options => options.UseInMemoryDatabase().ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
            return serviceCollection;
        }

        private static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IInsuredClientsRepository, InsuredClientsRepository>();
        }
    }
}
