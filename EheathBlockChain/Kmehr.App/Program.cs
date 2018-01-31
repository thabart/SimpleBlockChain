using be.business.connector.common;
using be.business.connector.recipe.prescriber.mock;
using System.IO;
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
    }
}
