namespace Kmehr.Core
{
    internal static class Constants
    {
        #region Enumeration values

        public static class KmehrIdentifiers
        {
            public const string IDKMEHR = "ID-KMEHR";
            public const string IDFOLDER = "ID-FOLDER";
            public const string IDTRANSACTION = "ID-TRANSACTION";
            public const string IDHEADING = "ID-HEADING";
            public const string IDITEM = "ID-ITEM";
            public const string IDHCPARTY = "ID-HCPARTY";
            public const string IDPATIENT = "ID-PATIENT";
            public const string LOCAL = "LOCAL";
        }

        public static class KmehrSenderQualifications
        {
            public const string CDHCPARTY = "CD-HCPARTY";
            public const string CDVMPGROUP = "CD-VMPGROUP";
            public const string CDSTANDARD = "CD-STANDARD";
            public const string CDSEX = "CD-SEX";
            public const string CDTRANSACTION = "CD-TRANSACTION";
            public const string CDFEDCOUNTRY = "CD-FED-COUNTRY";
            public const string CDADDRESS = "CD-ADDRESS";
        }

        public static class KmehrSexTypeNames
        {
            public const string M = "male";
            public const string F = "female";
        }

        public static class KmehrMessageNames
        {
            public const string Header = "header";
            public const string Folder = "folder";
        }

        public static class KmehrAddressTypeNames
        {
            public const string Work = "work";
        }

        #endregion

        #region DTO values

        public static class KmehrHcPartyNames
        {
            public const string Id = "id";
            public const string Cd = "cd";
            public const string Name = "name";
            public const string Telecom = "telecom";
            public const string Firstname = "firstname";
            public const string Lastname = "lastname";
            public const string Address = "address";
        }

        public static class KmehrHeaderNames
        {
            public const string Standard = "standard";
            public const string Id = "id";
            public const string Date = "date";
            public const string Time = "time";
            public const string Sender = "sender";
            public const string Recipient = "recipient";
        }

        public static class KmehrSenderNames
        {
            public const string HcParty = "hcparty";
        }

        public static class KmehrTelecomNames
        {
            public const string Cd = "cd";
            public const string TelecomNumber = "telecomnumber";
        }

        public static class KmehrFolderNames
        {
            public const string Id = "id";
            public const string Patient = "patient";
            public const string Transaction = "transaction";
        }

        public static class KmehrSexNames
        {
            public const string Id = "id";
        }

        public static class KmehrDateNames
        {
            public const string Date = "date";
        }

        public static class KmehrPatientNames
        {
            public const string Id = "id";
            public const string FirstName = "firstname";
            public const string LastName = "lastname";
            public const string BirthDate = "birthdate";
            public const string Sex = "sex";
        }

        public static class KmehrIdNames
        {
            public const string S = "S";
            public const string Sv = "SV";
            public const string Sl = "SL";
        }

        public static class KmehrTransactionNames
        {
            public const string Date = "date";
            public const string Time = "time";
            public const string Author = "author";
            public const string IsComplete = "iscomplete";
            public const string IsValidated = "isvalidated";
        }

        public static class KmehrTransactionTypes
        {
            public const string Pharmaceuticalprescription = "pharmaceuticalprescription";
        }

        public static class KmehrAddressNames
        {
            public const string Country = "country";
            public const string Zip = "zip";
            public const string City = "city";
            public const string Street = "street";
            public const string HouseNumber = "housenumber";
        }

        public static class KmehrCommonNames
        {
            public const string Id = "id";
            public const string Cd = "cd";
        }

        #endregion
    }
}
