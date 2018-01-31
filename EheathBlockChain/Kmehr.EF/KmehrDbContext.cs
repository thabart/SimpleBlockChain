using Kmehr.EF.Mappings;
using Kmehr.EF.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Kmehr.EF
{
    internal class KmehrDbContext : DbContext
    {
        public KmehrDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            try
            {
                Database.ExecuteSqlCommand("SET TRANSACTION ISOLATION LEVEL READ COMITTED;");
            } catch(Exception) { }
        }

        public virtual DbSet<HealthCarePartyType> HealthCarePartyTypes { get; set; }
        public virtual DbSet<Language> Languages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddHeathCareTypeMapping()
                .AddLanguageMapping()
                .AddTranslationMapping();
        }
    }
}
