using Microsoft.Extensions.DependencyInjection;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Connectors;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Scripts;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Validators;
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
            serviceCollection.AddTransient<ITransactionBuilder, TransactionBuilder>();
            serviceCollection.AddTransient<IScriptBuilder, ScriptBuilder>();
            serviceCollection.AddTransient<IAssemblyHelper, AssemblyHelper>();
            serviceCollection.AddTransient<IBlockChainFactory, BlockChainFactory>();
            serviceCollection.AddTransient<ITransactionValidator, TransactionValidator>();
            serviceCollection.AddTransient<IBlockValidator, BlockValidator>();
            serviceCollection.AddTransient<IScriptInterpreter, ScriptInterpreter>();
            serviceCollection.AddTransient<ITransactionHelper, TransactionHelper>();
            serviceCollection.AddTransient<IMessageCoordinator, MessageCoordinator>();
            serviceCollection.AddTransient<ISmartContractFactory, SmartContractFactory>();
            serviceCollection.AddSingleton<IBlockChainStore, BlockChainStore>();
            serviceCollection.AddSingleton<ISmartContractStore, SmartContractStore>();
            return serviceCollection;
        }
    }
}
