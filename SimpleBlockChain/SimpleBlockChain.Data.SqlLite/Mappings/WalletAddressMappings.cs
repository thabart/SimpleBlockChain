using Microsoft.EntityFrameworkCore;
using SimpleBlockChain.Data.Sqlite.Models;
using System;

namespace SimpleBlockChain.Data.Sqlite.Mappings
{
    internal static class WalletAddressMappings
    {
        public static ModelBuilder AddWalletAddressMapping(this ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<WalletAddress>()
                .ToTable("walletAddresses")
                .HasKey(w => w.Hash);
            return modelBuilder;
        }
    }
}
