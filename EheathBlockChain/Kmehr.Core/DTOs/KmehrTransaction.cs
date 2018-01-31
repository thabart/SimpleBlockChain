using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public enum KmehrTransactionTypes
    {
        Pharmaceuticalprescription
    }

    public class KmehrTransaction
    {
        private Dictionary<KmehrTransactionTypes, string> _mappingEnumToTransactionStr = new Dictionary<KmehrTransactionTypes, string>
        {
            { KmehrTransactionTypes.Pharmaceuticalprescription, Constants.KmehrTransactionTypes.Pharmaceuticalprescription }
        };

        public KmehrTransaction(KmehrTransactionTypes transactionType)
        {
            Id = new KmehrId(Constants.KmehrIdentifiers.IDKMEHR,  "1.0", "1");
            Cd = new KmehrId(Constants.KmehrSenderQualifications.CDTRANSACTION, "1.9", _mappingEnumToTransactionStr[transactionType]);
            IsComplete = false;
            IsValidated = false;
        }

        [XmlElement(Constants.KmehrCommonNames.Id)]
        public KmehrId Id { get; private set; }
        [XmlElement(Constants.KmehrCommonNames.Cd)]
        public KmehrId Cd { get; set; }
        [XmlElement(Constants.KmehrTransactionNames.Date)]
        public DateTime Date { get; set; }
        [XmlElement(Constants.KmehrTransactionNames.Time)]
        public DateTime Time { get; set; }
        [XmlElement(Constants.KmehrTransactionNames.Author)]
        public KmehrHcParty Author { get; set; }
        [XmlElement(Constants.KmehrTransactionNames.IsComplete)]
        public bool IsComplete { get; set; }
        [XmlElement(Constants.KmehrTransactionNames.IsValidated)]
        public bool IsValidated { get; set; }
    }
}
