using Microsoft.EntityFrameworkCore;
using SimpleBlockChain.Data.Sqlite.Mappings;
using SimpleBlockChain.Data.Sqlite.Models;

namespace SimpleBlockChain.Data.Sqlite
{
    public class CurrentDbContext : DbContext
    {
        public CurrentDbContext(DbContextOptions dbContextOptions):base(dbContextOptions)
        {
        }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<SolidityContract> SolidityContracts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddWalletMapping()
                .AddSolidityContracts();
            base.OnModelCreating(modelBuilder);
        }
    }
}
