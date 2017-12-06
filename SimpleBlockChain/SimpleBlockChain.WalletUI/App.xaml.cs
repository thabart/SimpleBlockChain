using Microsoft.Extensions.DependencyInjection;
using SimpleBlockChain.WalletUI.Pages;
using SimpleBlockChain.WalletUI.ViewModels;
using System.Windows;
using SimpleBlockChain.Data.Sqlite;

namespace SimpleBlockChain.WalletUI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSqlite("Data Source=wallet.db");

            serviceCollection.AddTransient<HomePage>();
            serviceCollection.AddTransient<CreateWalletPage>();
            serviceCollection.AddTransient<AuthenticateWalletPage>();

            serviceCollection.AddTransient<HomePageViewModel>();
            serviceCollection.AddTransient<CreateWalletViewModel>();
            serviceCollection.AddTransient<AuthenticateWalletViewModel>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var currentDbContext = serviceScope.ServiceProvider.GetService<CurrentDbContext>();
                currentDbContext.Database.EnsureCreated();
            }

            MainWindow mainWindow = ActivatorUtilities.CreateInstance<MainWindow>(serviceProvider);
            mainWindow.Show();
        }
    }
}
