using System;
using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public class KmehrDate
    {
        public KmehrDate(DateTime date)
        {
            Date = date;
        }

        [XmlElement(Constants.KmehrDateNames.Date)]
        public DateTime Date { get; private set; }
    }
}
