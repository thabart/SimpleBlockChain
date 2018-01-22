using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace EheathBlockChain.OpenId
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .Build();
            var host = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    var httpsOptions = new HttpsConnectionFilterOptions
                    {
                        ServerCertificate = new X509Certificate2("SimpleIdServer.pfx")
                    };
                    options.UseHttps(httpsOptions);
                })
                .UseIISIntegration()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(configuration)
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}
