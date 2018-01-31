using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public class KmehrId
    {
        public KmehrId()
        {

        }

        public KmehrId(string s, string sv, string value)
        {
            S = s;
            Sv = sv;
            Value = value;
        }

        public KmehrId(string s, string sv, string value, string sl) : this(s, sv, value)
        {
            Sl = sl;
        }

        [XmlAttribute(Constants.KmehrIdNames.S)]
        public string S { get; set; }
        [XmlAttribute(Constants.KmehrIdNames.Sv)]
        public string Sv { get; set; }
        [XmlAttribute(Constants.KmehrIdNames.Sl)]
        public string Sl { get; set; }
        [XmlText]
        public string Value { get; set; }
    }
}
