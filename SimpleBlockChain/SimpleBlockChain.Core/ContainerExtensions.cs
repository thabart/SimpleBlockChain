using Microsoft.Extensions.DependencyInjection;
using SimpleBlockChain.Core.Factories;
using System;

namespace SimpleBlockChain.Core
{
    public static class ContainerExtensions
    {
        public static IServiceCollection AddCore(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.AddTransient<IRpcNodeFactory, RpcNodeFactory>();
            serviceCollection.AddTransient<INodeLauncherFactory, NodeLauncherFactory>();
            serviceCollection.AddTransient<IHttpClientFactory, HttpClientFactory>();
            return serviceCollection;
        }
    }
}
