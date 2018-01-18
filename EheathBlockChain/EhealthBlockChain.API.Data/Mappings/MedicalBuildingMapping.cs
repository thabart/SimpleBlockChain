using EhealthBlockChain.API.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EhealthBlockChain.API.Data.Mappings
{
    internal static class MedicalBuildingMapping
    {
        public static ModelBuilder AddMedicalBuilding(this ModelBuilder modelBuider)
        {
            if (modelBuider == null)
            {
                throw new ArgumentNullException(nameof(modelBuider));
            }

            modelBuider.Entity<MedicalBuilding>()
                .HasKey(i => i.Id);
            modelBuider.Entity<MedicalBuilding>()
                .HasMany(i => i.MedicalAssignments)
                .WithOne(i => i.MedicalBuilding)
                .HasForeignKey(i => i.MedicalBuildingId);
            return modelBuider;
        }
    }
}
