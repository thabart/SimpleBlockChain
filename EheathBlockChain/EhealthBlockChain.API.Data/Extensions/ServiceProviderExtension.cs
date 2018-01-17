using EhealthBlockChain.API.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace EhealthBlockChain.API.Data.Extensions
{
    public static class ServiceProviderExtension
    {
        public static IServiceProvider AddFakeData(this IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var currentDbContext = serviceScope.ServiceProvider.GetService<CurrentDbContext>();
                currentDbContext.Database.EnsureCreated();
                currentDbContext.InsuredClients.AddRange(new List<InsuredClient>
                {
                    new InsuredClient
                    {
                        FirstName = "thierry",
                        LastName = "habart",
                        NationalRegistrationNumber = "00001"
                    },
                    new InsuredClient
                    {
                        FirstName = "laetitia",
                        LastName = "buyse",
                        NationalRegistrationNumber = "00002"
                    }
                });
            }

            return serviceProvider;
        }
    }
}
