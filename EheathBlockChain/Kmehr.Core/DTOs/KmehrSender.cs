using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public class KmehrSender
    {
        [XmlElement(Constants.KmehrSenderNames.HcParty)]
        public List<KmehrHcParty> HcParties { get; set; }
    }
}
