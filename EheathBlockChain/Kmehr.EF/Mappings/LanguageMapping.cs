using Kmehr.EF.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Kmehr.EF.Mappings
{
    internal static class LanguageMapping
    {
        public static ModelBuilder AddLanguageMapping(this ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<Language>()
                .ToTable("languages");
            modelBuilder.Entity<Language>()
                .HasKey(l => l.Id);
            modelBuilder.Entity<Language>()
                .HasMany(l => l.Translations)
                .WithOne(t => t.Language)
                .HasForeignKey(t => t.LanguageId);
            return modelBuilder;
        }
    }
}
