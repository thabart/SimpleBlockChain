using Kmehr.EF.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Kmehr.EF.Mappings
{
    internal static class TranslationMapping
    {
        public static ModelBuilder AddTranslationMapping(this ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<Translation>()
                .ToTable("translations");
            modelBuilder.Entity<Translation>()
                .HasKey(t => new { t.HealthCarePartyTypeId, t.LanguageId });
            return modelBuilder;
        }
    }
}
