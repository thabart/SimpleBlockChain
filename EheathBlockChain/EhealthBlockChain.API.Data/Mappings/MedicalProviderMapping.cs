using EhealthBlockChain.API.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EhealthBlockChain.API.Data.Mappings
{
    internal static class MedicalProviderMapping
    {
        public static ModelBuilder AddMedicalProvider(this ModelBuilder modelBuider)
        {
            if (modelBuider == null)
            {
                throw new ArgumentNullException(nameof(modelBuider));
            }

            modelBuider.Entity<MedicalProvider>()
                .HasKey(i => i.InamiCode);
            modelBuider.Entity<MedicalProvider>()
                .HasMany(i => i.MedicalAssignments)
                .WithOne(i => i.MedicalProvider)
                .HasForeignKey(i => i.MedicalProviderId);
            return modelBuider;
        }
    }
}
