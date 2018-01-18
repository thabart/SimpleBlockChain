using EhealthBlockChain.API.Data.Mappings;
using EhealthBlockChain.API.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EhealthBlockChain.API.Data
{
    internal sealed class CurrentDbContext : DbContext
    {
        public CurrentDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            try
            {
                Database.ExecuteSqlCommand("SET TRANSACTION ISOLATION LEVEL READ COMITTED;");
            }
            catch { }
        }

        public DbSet<InsuredClient> InsuredClients { get; set; }
        public DbSet<MedicalProvider> MedicalProviders { get; set; }
        public DbSet<MedicalBuilding> MedicalBuildings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddInsuredClient()
                .AddMedicalAssignment()
                .AddMedicalBuilding()
                .AddMedicalProvider();
            base.OnModelCreating(modelBuilder);
        }
    }
}
