using Microsoft.EntityFrameworkCore;
using SimpleBlockChain.Data.Sqlite.Models;
using System;

namespace SimpleBlockChain.Data.Sqlite.Mappings
{
    internal static class SolidityContractMappings
    {
        public static ModelBuilder AddSolidityContracts(this ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<SolidityContract>()
                .ToTable("contracts")
                .HasKey(w => w.Address);
            modelBuilder.Entity<SolidityContract>()
                .HasMany(c => c.Filters)
                .WithOne(c => c.SmartContract)
                .HasForeignKey(c => c.SmartContractAddress);
            return modelBuilder;
        }
    }
}
