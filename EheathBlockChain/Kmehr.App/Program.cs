using Kmehr.Core.Helpers;

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
            var prescriptionPayload = System.Text.Encoding.UTF8.GetBytes(prescriptionContent);
            var compressedPrescriptionPayload = IOHelpers.Compress(prescriptionPayload);

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
    }
}
