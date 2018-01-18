using EhealthBlockChain.API.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EhealthBlockChain.API.Data.Mappings
{
    internal static class MedicalAssignmentMappingcs
    {
        public static ModelBuilder AddMedicalAssignment(this ModelBuilder modelBuider)
        {
            if (modelBuider == null)
            {
                throw new ArgumentNullException(nameof(modelBuider));
            }

            modelBuider.Entity<MedicalAssignment>()
                .HasKey(i => new
                {
                    i.MedicalBuildingId,
                    i.MedicalProviderId
                });
            return modelBuider;
        }
    }
}
