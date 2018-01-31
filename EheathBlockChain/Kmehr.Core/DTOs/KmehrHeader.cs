using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    /// <summary>
    /// Specify the routing of the message.
    /// </summary>
    public class KmehrHeader
    {
        public KmehrHeader()
        {
            Standard = new KmehrId(Constants.KmehrSenderQualifications.CDSTANDARD, "1.18", "20160601");
            Ids = new List<KmehrId>
            {
                new KmehrId(Constants.KmehrIdentifiers.IDKMEHR , "1.0", "19006951001.20090110090000000"),
                new KmehrId(Constants.KmehrIdentifiers.LOCAL, "1.0", "6b7e77b8-b987-4f87-9b86-403af535e9c9", "EHEALTH")
            };
            var now = DateTime.UtcNow;
            Date = now.ToString("yyyy-MM-dd");
            Time = now.ToString("HH:mm:ss");
        }

        [XmlElement(Constants.KmehrHeaderNames.Standard)]
        public KmehrId Standard { get; set; }
        [XmlElement(Constants.KmehrHeaderNames.Id)]
        public List<KmehrId> Ids { get; set; }
        [XmlElement(Constants.KmehrHeaderNames.Date)]
        public string Date { get; set; }
        [XmlElement(Constants.KmehrHeaderNames.Time)]
        public string Time { get; set; }
        [XmlElement(Constants.KmehrHeaderNames.Sender)]
        public KmehrHcParties Sender { get; set; }
        [XmlElement(Constants.KmehrHeaderNames.Recipient)]
        public KmehrHcParties Recipient { get; set; }
    }
}
