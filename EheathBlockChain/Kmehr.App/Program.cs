using be.ehealth.technicalconnector.config;
using be.ehealth.technicalconnector.session;

namespace Kmehr.App
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Can connect with the EID.
            var configurationFile = @"c:\Project\SimpleBlockChain\EheathBlockChain\conf\be.ehealth.technicalconnector.properties";
            ConfigFactory.setConfigLocation(configurationFile);
            var sessionMgt = Session.getInstance();
            sessionMgt.unloadSession();
            var sessionItem = sessionMgt.createSessionEidOnly();
        }
    }
}
