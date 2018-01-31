using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Kmehr.Core.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kmehr.Core.Tests
{
    [TestClass]
    public class KmehrMessageTest
    {
        [TestMethod]
        public void WhenBuildKmehrMessage()
        {
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
