using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.EF.Resources
{
    public sealed class CdHcPartyDescription
    {
        [XmlAttribute("L")]
        public string Language { get; set; }
        [XmlText]
        public string Value { get; set; }
    }

    public sealed class CdHcPartyValue
    {
        [XmlElement("CODE")]
        public string Code { get; set; }
        [XmlElement("DESCRIPTION")]
        public List<CdHcPartyDescription> Descriptions { get; set; }
    }
        
    [Serializable]
    [XmlRoot("kmehr-cd")]
    public sealed class CdHcParty
    {
        [XmlElement("VERSION")]
        public string Version { get; set; }
        [XmlElement("DATE")]
        public string Date { get; set; }
        [XmlElement("STATUS")]
        public string Status { get; set; }
        [XmlElement("NAME")]
        public string Name { get; set; }
        [XmlElement("VALUE")]
        public List<CdHcPartyValue> Values { get; set; }
    }
}
