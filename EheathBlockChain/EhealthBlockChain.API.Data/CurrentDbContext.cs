using EhealthBlockChain.API.Data.Mappings;
using EhealthBlockChain.API.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EhealthBlockChain.API.Data
{
    internal sealed class CurrentDbContext : DbContext
    {
        public CurrentDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<InsuredClient> InsuredClients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddInsuredClient();
            base.OnModelCreating(modelBuilder);
        }
    }
}
