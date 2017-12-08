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
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Extensions;

namespace SimpleBlockChain.WalletUI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSqlite("Data Source=genesis.db");

            serviceCollection.AddTransient<HomePage>();
            serviceCollection.AddTransient<CreateWalletPage>();
            serviceCollection.AddTransient<AuthenticateWalletPage>();
            serviceCollection.AddTransient<WalletPage>();

            serviceCollection.AddTransient<HomePageViewModel>();
            serviceCollection.AddTransient<CreateWalletViewModel>();
            serviceCollection.AddTransient<WalletPageViewModel>();
            serviceCollection.AddSingleton<IDialogCoordinator>(DialogCoordinator.Instance);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var currentDbContext = serviceScope.ServiceProvider.GetService<CurrentDbContext>();
                currentDbContext.Database.EnsureCreated();
                var walletAggregate = new WalletAggregate
                {
                    Name = "genesis",
                    Addresses = new List<WalletAggregateAddress>
                    {
                        new WalletAggregateAddress
                        {
                            Hash = "82c48a4388bbf1b85dca2b6f0793a8ff7774a8a7",
                            Key = Key.Deserialize(new BigInteger("66661394595692466950200829442443674598224300882267065208709422638481412972116609477112206002430829808784107536250360432119209033266013484787698545014625057"), new BigInteger("43102461949956883352376427470284148089747996528740865531180015053863743793176"))
                        }
                    }
                };
                var repo = serviceProvider.GetService<IWalletRepository>();
                repo.Add(walletAggregate, "zvhab8rijwl7vwma".ToSecureString());
            }

            MainWindow mainWindow = ActivatorUtilities.CreateInstance<MainWindow>(serviceProvider);
            mainWindow.Show();
        }
    }
}
