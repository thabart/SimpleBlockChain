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
            const string hcTypeCode = "persphysician";
            // 1. Register all the dependencies.
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddKmehrInMemory();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.EnsureSeedData();

            // use : physician
            // 2. Get the cd-hcparty
            var repository = serviceProvider.GetService<IHealthCarePartyTypeRepository>();
            var hcType = await repository.Get(hcTypeCode);

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
