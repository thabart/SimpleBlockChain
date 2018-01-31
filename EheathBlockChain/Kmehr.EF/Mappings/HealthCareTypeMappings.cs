﻿using Kmehr.EF.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Kmehr.EF.Mappings
{
    internal static class HealthCareTypeMappings
    {
        public static ModelBuilder AddHeathCareTypeMapping(this ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<HealthCarePartyType>()
                .ToTable("healthcaretypes");
            modelBuilder.Entity<HealthCarePartyType>()
                .HasKey(h => h.Code);
            return modelBuilder;
        }
    }
}
