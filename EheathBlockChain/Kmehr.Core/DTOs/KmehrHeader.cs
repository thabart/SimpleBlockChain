using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public class KmehrHeader
    {
        [XmlElement(Constants.KmehrHeaderNames.Id)]
        public List<KmehrId> Id { get; set; }
        [XmlElement(Constants.KmehrHeaderNames.Date)]
        public DateTime Date { get; set; }
        [XmlElement(Constants.KmehrHeaderNames.Time)]
        public DateTime Time { get; set; }
    }
}
