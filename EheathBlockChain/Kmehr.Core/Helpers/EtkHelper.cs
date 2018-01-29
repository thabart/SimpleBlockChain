using Kmehr.Core.Kgss;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kmehr.Core.Helpers
{
    public class EtkHelper
    {
        private const string KGSS_ID = "0809394427";
        private const string RECIPE_ID = "0823257311";
        private const string PCDH_ID = "0406753266";

        private readonly EncryptionHelper _encryptionHelper;

        public EtkHelper(EncryptionHelper encryptionHelper)
        {
            _encryptionHelper = encryptionHelper;
        }

        public List<string> GetKgssEtk()
        {
            // return getEtks(KgssIdentifierType.CBE, KGSS_ID, "KGSS");
            return null;
        }

        public List<string> GetSystemEtk()
        {

            return null;
        }

        public List<string> GetEtks(KgssIdentifierTypes identifierType, string identifierValue)
        {
            return null;
        }

        public List<string> GetEtks(KgssIdentifierTypes identifierType, long identifierValue, string applicationId)
        {
            switch(identifierType)
            {
                case KgssIdentifierTypes.CBE:

                    break;
                case KgssIdentifierTypes.SSIN:

                    break;
                case KgssIdentifierTypes.NIHIIPHARMACY:

                    break;
            }

            return null;
        }

        public List<string> GetEtks(KgssIdentifierTypes identifierType, string identifierValue, string applicationId)
        {
            /*
            String etkCacheId = identifierType + "/" + identifierValue + "/" + application;
            if (etkCache.containsKey(etkCacheId))
            {
                LOG.info("ETK retrieved from the cache : " + etkCacheId);
                return etksCache.get(etkCacheId);
            }

            List<EncryptionToken> encryptionTokens = getEtksFromDepot(identifierType, identifierValue, application);
            etksCache.put(etkCacheId, encryptionTokens);
            return encryptionTokens;
            */
            return null;
        }

        private static string LongToString(long id, int numberOfDigits)
        {
            var buffer = new StringBuilder(id.ToString());
            var delta = numberOfDigits - buffer.Length;
            if (delta < 0)
            {
                throw new ArgumentException("numberOfDigits < input length");
            }

            if (delta == 0)
            {
                return buffer.ToString();
            }

            for(; delta > 0; --delta)
            {
                buffer.Append("0");
            }

            return buffer.ToString();
        }
    }
}
