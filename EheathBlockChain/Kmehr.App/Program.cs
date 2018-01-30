using be.business.connector.common;
using be.business.connector.core.utils;
using be.business.connector.recipe.prescriber.mock;
using be.business.connector.session;
using be.ehealth.technicalconnector.config;
using Kmehr.Core.Etk;
using Kmehr.Core.Kgss;
using Kmehr.Core.STS;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Kmehr.App
{
    class Program
    {
        static void Main(string[] args)
        {
            const string propertyfile = "../../conf/connector-client.properties";
            const string vslidationFile = "../../conf/validation.properties";
            ApplicationConfig.getInstance().initialize(propertyfile, vslidationFile);
            var moduleInstance = new PrescriberIntegrationModuleMock();
            // 1. Create a prescription.
            var isFeedbackChecked = true;
            var patientId = "81112623980";
            var samplePrescription = Path.Combine(Directory.GetCurrentDirectory(), "samples/sample-prescription.xml");
            var prescriptionPayload = Encoding.UTF8.GetBytes(File.ReadAllText(samplePrescription));
            var prescriptionType = "P0";
            var rid = moduleInstance.createPrescription(isFeedbackChecked, patientId, prescriptionPayload, prescriptionType);
            // 2. Get a prescription.
            var getPrescriptionResult = moduleInstance.getPrescription(rid);
            string s = "";
            // DEVELOP A WEBSITE TO CREATE A PRESCRIPTION.
        }

        private static void CreateSession()
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
        }

        private static void GetNewKey()
        {
            float patientId = 81112623980;
            const string prescriptionType = "P0";

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
            const string identificationKeyStore = "";
            const string privateKeyAlias = "";
            // 1. Load the identification keys.

            var samlNameIdentifier = new SamlNameIdentifier
            {

            };
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

        private static void TryToAuthenticate()
        {
            const string propertyfile = "../../conf/connector-client.properties";
            const string vslidationFile = "../../conf/validation.properties";
            const string niss = "81112623980";
            const string password = "Password1";
            // 1. Load the configuration.
            var applicationConfig = ApplicationConfig.getInstance();
            applicationConfig.initialize(propertyfile, vslidationFile);
            // 2. Load the properties.
            var handler = PropertyHandler.getInstance();
            var properties = PropertyHandler.getInstance().getPropertiesCopy();
            // 3. Create a session.
            // SessionUtil.createSession(SessionType.EID_SESSION, properties, null, null); // NO BE ID CARD.
            // ConfigFactory.setConfigLocation(@"c:\Projects\SimpleBlockChain\EheathBlockChain\conf\connector-client.properties");
            var location = ConfigFactory.getConfigLocation();
            // SessionUtil.createSession(SessionType.MANDATE_SESSION, properties, null, null);
            SessionUtil.createSession(SessionType.FALLBACK_SESSION, properties, niss, password);
        }
    }
}
