using Microsoft.EntityFrameworkCore;
using SimpleBlockChain.Data.Sqlite.Models;
using System;

namespace SimpleBlockChain.Data.Sqlite.Mappings
{
    internal static class WalletMappings
    {
        public static ModelBuilder AddWalletMapping(this ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<Wallet>()
                .ToTable("wallets")
                .HasKey(w => w.Name);
            modelBuilder.Entity<Wallet>()
                .HasMany(w => w.Addresses)
                .WithOne(wa => wa.Wallet)
                .HasForeignKey(wa => wa.WalletName);
            return modelBuilder;
        }
    }
}
