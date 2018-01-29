using Kmehr.Core.Common;
using Kmehr.Core.Etk;
using Kmehr.Core.Helpers;
using Kmehr.Core.Kgss;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Kmehr.App
{
    class Program
    {
        static void Main(string[] args)
        {
            // CreatePrescription : https://services-int.ehealth.fgov.be/pilot/Recip-e/v1/Prescriber_v1
            // NewKey             : https://services-int.ehealth.fgov.be/Kgss/v1
            var prescriptionContent = ""; // TODO : Replace by the kmehr message.
            var prescriptionType = "P0";
            var patientId = 72081061175;
            var credentialTypes = new List<string>
            {
                "urn:be:fgov:certified-namespace:ehealth,urn:be:fgov:person:ssin:ehealth:1.0:doctor:nihii11",
                "urn:be:fgov:certified-namespace:ehealth,urn:be:fgov:ehealth:1.0:pharmacy:nihii-number:recognisedpharmacy:boolean,true",
                "urn:be:fgov:identification-namespace,urn:be:fgov:person:ssin,%PATIENT_ID%"
            };

            var directory = Directory.GetCurrentDirectory();
            var jksPath = Path.Combine(directory, "caCertificateKeystore.jks");
            string password = "Password1";
            var certificate = new X509Certificate2(jksPath, password);
            string s = "";
            // TRY TO GET THE NEW KEY.

            // 1. RETRIEVE THE STS TOKEN.
            /*
            var prescriptionPayload = System.Text.Encoding.UTF8.GetBytes(prescriptionContent);
            var compressedPrescriptionPayload = IOHelpers.Compress(prescriptionPayload);
            */
            // TODO:  Get the key.
            /*
             * 
   public GetKeyResponseContent getKey(GetKeyRequestContent request, Credential encryption, Credential service, Element samlAssertion, Map<String, PrivateKey> decryptionKeys, byte[] etkKGSS) throws TechnicalConnectorException {
      SAMLToken token = SAMLTokenFactory.getInstance().createSamlToken(samlAssertion, service);
      KgssMessageBuilder builder = new KgssMessageBuilderImpl(etkKGSS, encryption, decryptionKeys);
      GetKeyRequest sealedRequest = builder.sealGetKeyRequest(request);
      GenericRequest genericRequest = ServiceFactory.getKGSSServiceSecured(token);
      genericRequest.setPayload((Object)sealedRequest);

      try {
         GetKeyResponse response = (GetKeyResponse)be.ehealth.technicalconnector.ws.ServiceFactory.getGenericWsSender().send(genericRequest).asObject(GetKeyResponse.class);
         checkReplyStatus(response.getStatus().getCode());
         this.checkErrorMessages(response.getErrors());
         return builder.unsealGetKeyResponse(response);
      } catch (SOAPException var12) {
         throw new TechnicalConnectorException(TechnicalConnectorExceptionValues.ERROR_WS, new Object[]{var12.getMessage(), var12});
      }
   }*/
            // COMPRESS THE PAYLOAD.
        }

        private static void GetNewKey()
        {
            var request = new GetNewKeyRequestContent();
        }

        private static void GetEtk()
        {
            var request = new GetEtkRequest();
            var searchCriteriaType = new SearchCriteriaType();
            var lstIdentifiers = new List<EtkIdentifierType>();
            var identifier = new EtkIdentifierType();            
        }

        private static void GetSamlToken()
        {
            // endpoint.sts : https://services-int.ehealth.fgov.be/IAM/Saml11TokenService/Legacy/v1

            /*
             * 
      Validate.notNull(headerCredential, "Parameter headerCredential is not nullable");
      Validate.notNull(bodyCredential, "Parameter bodyCredential is not nullable");
      List<SAMLAttributeDesignator> designators = SAMLConfigHelper.getSAMLAttributeDesignators("sessionmanager.samlattributedesignator");
      List<SAMLAttribute> attributes = SAMLConfigHelper.getSAMLAttributes("sessionmanager.samlattribute");
      STSService sts = STSServiceFactory.getInstance();
      Element assertion = sts.getToken(headerCredential, bodyCredential, attributes, designators, "urn:oasis:names:tc:SAML:1.0:cm:holder-of-key", validityHours);
      return SAMLTokenFactory.getInstance().createSamlToken(assertion, bodyCredential);*/
        }

        private static void GetKey()
        {
            
        }
    }
}
