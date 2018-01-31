using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public enum KmehrSexTypes
    {
        M,
        F
    }

    public class KmehrSex
    {
        private Dictionary<KmehrSexTypes, string> _mappingEnumToSexStr = new Dictionary<KmehrSexTypes, string>
        {
            { KmehrSexTypes.F, Constants.KmehrSexTypeNames.F },
            { KmehrSexTypes.M, Constants.KmehrSexTypeNames.M }
        };

        public KmehrSex(KmehrSexTypes type)
        {
            Id = new KmehrId(Constants.KmehrSenderQualifications.CDSEX, "1.1", _mappingEnumToSexStr[type]);
        }

        [XmlElement(Constants.KmehrSexNames.Id)]
        public KmehrId Id { get; private set; }
    }
}
