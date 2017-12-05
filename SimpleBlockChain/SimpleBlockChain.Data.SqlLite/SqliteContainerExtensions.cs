using Microsoft.Extensions.DependencyInjection;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Data.Sqlite.Repositories;
using System;

namespace SimpleBlockChain.Data.Sqlite
{
    public static class SqliteContainerExtensions
    {
        public static IServiceCollection AddSqlite(IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.AddTransient<IWalletRepository, WalletRepository>();
            return serviceCollection;
        }
    }
}
