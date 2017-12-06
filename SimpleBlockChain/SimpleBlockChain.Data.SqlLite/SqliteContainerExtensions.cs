using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Data.Sqlite.Repositories;
using System;

namespace SimpleBlockChain.Data.Sqlite
{
    public static class SqliteContainerExtensions
    {
        public static IServiceCollection AddInMemory(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            RegisterServices(serviceCollection);
            serviceCollection.AddEntityFramework()
                            .AddDbContext<CurrentDbContext>(options => options.UseInMemoryDatabase().ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
            return serviceCollection;
        }

        public static IServiceCollection AddSqlite(this IServiceCollection serviceCollection, string connectionString)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            RegisterServices(serviceCollection);
            serviceCollection.AddEntityFramework()
                .AddDbContext<CurrentDbContext>(options =>
                {
                    options.UseSqlite(connectionString);
                });

            return serviceCollection;
        }

        private static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IWalletRepository, WalletRepository>();
        }
    }
}
