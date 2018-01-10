using Microsoft.Extensions.DependencyInjection;
using SimpleBlockChain.WalletUI.Pages;
using SimpleBlockChain.WalletUI.ViewModels;
using System.Windows;
using SimpleBlockChain.Data.Sqlite;
using MahApps.Metro.Controls.Dialogs;
using SimpleBlockChain.Core.Aggregates;
using System.Collections.Generic;
using SimpleBlockChain.Core.Crypto;
using Org.BouncyCastle.Math;
using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.WalletUI.UserControls;

namespace SimpleBlockChain.WalletUI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCore();
            serviceCollection.AddSqlite("Data Source=genesis.db");

            serviceCollection.AddTransient<HomePage>();
            serviceCollection.AddTransient<CreateWalletPage>();
            serviceCollection.AddTransient<AuthenticateWalletPage>();
            serviceCollection.AddTransient<WalletPage>();
            serviceCollection.AddTransient<BlockChainInformation>();
            serviceCollection.AddTransient<WalletInformation>();
            serviceCollection.AddTransient<MemoryPoolInformation>();
            serviceCollection.AddTransient<SmartContractPage>();

            serviceCollection.AddTransient<HomePageViewModel>();
            serviceCollection.AddTransient<CreateWalletViewModel>();
            serviceCollection.AddTransient<WalletPageViewModel>();
            serviceCollection.AddTransient<WalletInformationViewModel>();
            serviceCollection.AddSingleton<IDialogCoordinator>(DialogCoordinator.Instance);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var currentDbContext = serviceScope.ServiceProvider.GetService<CurrentDbContext>();
                currentDbContext.Database.EnsureCreated();
                var genesisWalletAggregate = new WalletAggregate
                {
                    Name = "genesis",
                    Addresses = new List<WalletAggregateAddress>
                    {
                        new WalletAggregateAddress
                        {
                            Hash = "12M1Wng2vRCT7z3uZcjDYb5i9bJDx4ZEKyVMhEq8",
                            Network = Networks.MainNet,
                            Key = Key.Deserialize(new BigInteger("66661394595692466950200829442443674598224300882267065208709422638481412972116609477112206002430829808784107536250360432119209033266013484787698545014625057"), new BigInteger("43102461949956883352376427470284148089747996528740865531180015053863743793176"))
                        }
                    }
                };
                var clientWalletAggregate = new WalletAggregate
                {
                    Name = "client",
                    Addresses = new List<WalletAggregateAddress>
                    {
                        new WalletAggregateAddress
                        {
                            Hash = "12K5LnVWKCu9QGyB39uGAgVSAfBs33PKS96HSL93",
                            Network = Networks.MainNet,
                            Key = Key.Deserialize(new BigInteger("55821205064713516294703127430400616105539980828115464481216494737343536494861392791366661233462519462101585894103124424523076975002332234845254777599135465"), new BigInteger("12865140029298721655663530581243123640092469699773563307406591049514067995825"))
                        }
                    }
                };
                
                var repo = serviceProvider.GetService<IWalletRepository>();
                var fact = serviceProvider.GetService<IBlockChainFactory>();
                repo.Add(clientWalletAggregate, "password".ToSecureString());
                repo.Add(genesisWalletAggregate, "zvhab8rijwl7vwma".ToSecureString());
            }

            MainWin mainWindow = ActivatorUtilities.CreateInstance<MainWin>(serviceProvider);
            mainWindow.Show();           
        }
    }
}
