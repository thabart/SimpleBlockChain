using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Kmehr.Core.DTOs;
using Kmehr.Core.Repositories;
using Kmehr.EF;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kmehr.Core.Tests
{
    [TestClass]
    public class KmehrMessageTest
    {                       
        [TestMethod]
        public async Task WhenBuildKmehrMessage()
        {
            const string hcTypeCodePhysician = "persphysician";
            const string hcTypeCodeApplication = "application";
            const string nidhiNumberPhysician = "19006951001";
            // 1. Register all the dependencies.
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddKmehrInMemory();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.EnsureSeedData();
            
            // 2. Get the cd-hcparty (médecin + application).
            var repository = serviceProvider.GetService<IHealthCarePartyTypeRepository>();
            var hcTypePhysician = await repository.Get(hcTypeCodePhysician);
            var hcTypeApplcation = await repository.Get(hcTypeCodeApplication);

            // 3.1 Build the hc-parties (physician)
            var physicianParty = new KmehrHcParty(hcTypePhysician.Code, nidhiNumberPhysician);
            physicianParty.FirstName = "Donald";
            physicianParty.Lastname = "Duck";

            // 3.2 Build the hc-parties (software)
            var softwareParty = new KmehrHcParty(hcTypeApplcation.Code);

            var header = new KmehrHeader();
            var medParty = new KmehrHcParty();
            var applicationParty = new KmehrHcParty();
            var serializer = new XmlSerializer(typeof(KmehrHeader));
            string xml = null;
            using (var strWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(strWriter))
                {
                    serializer.Serialize(xmlWriter, header);
                    xml = strWriter.ToString();
                }
            }

            string s = "";
        }
    }
}
