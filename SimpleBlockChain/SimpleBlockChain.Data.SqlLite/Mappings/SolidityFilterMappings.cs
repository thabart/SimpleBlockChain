using Microsoft.EntityFrameworkCore;
using SimpleBlockChain.Data.Sqlite.Models;
using System;

namespace SimpleBlockChain.Data.Sqlite.Mappings
{
    internal static class SolidityFilterMappings
    {
        public static ModelBuilder AddFilters(this ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<SolidityFilter>()
                .ToTable("filters")
                .HasKey(w => w.Id);
            return modelBuilder;
        }
    }
}
