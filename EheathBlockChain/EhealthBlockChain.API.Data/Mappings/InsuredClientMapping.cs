using EhealthBlockChain.API.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EhealthBlockChain.API.Data.Mappings
{
    internal static class InsuredClientMapping
    {
        public static ModelBuilder AddInsuredClient(this ModelBuilder modelBuider)
        {
            if (modelBuider == null)
            {
                throw new ArgumentNullException(nameof(modelBuider));
            }

            modelBuider.Entity<InsuredClient>()
                .HasKey(i => i.NationalRegistrationNumber);
            return modelBuider;
        }
    }
}
