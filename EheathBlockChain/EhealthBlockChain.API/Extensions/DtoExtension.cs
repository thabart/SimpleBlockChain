using EhealthBlockChain.API.Core.Commands;
using EhealthBlockChain.API.Core.Responses;
using Newtonsoft.Json.Linq;
using System;

namespace EhealthBlockChain.API.Extensions
{
    public static class DtoExtension
    {
        public static SearchInsuredClientsCommand GetSearchInsuredClients(this JObject jObj)
        {
            if (jObj == null)
            {
                throw new ArgumentNullException(nameof(jObj));
            }

            return new SearchInsuredClientsCommand
            {
                FirstName = jObj.TryGetString(Constants.InsuredClientNames.FirstName),
                LastName = jObj.TryGetString(Constants.InsuredClientNames.LastName),
                NationalRegistrationNumber = jObj.TryGetString(Constants.InsuredClientNames.NationalRegistrationNumber)
            };
        }

        public static JToken ToDto(this SearchInsuredClientsResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var result = new JArray();
            if (response.InsuredClients != null)
            {
                foreach(var insuredClient in response.InsuredClients)
                {
                    var jObj = new JObject();
                    jObj.Add(Constants.InsuredClientNames.FirstName, insuredClient.FirstName);
                    jObj.Add(Constants.InsuredClientNames.LastName, insuredClient.LastName);
                    jObj.Add(Constants.InsuredClientNames.NationalRegistrationNumber, insuredClient.NationalRegistrationNumber);
                    result.Add(jObj);
                }
            }

            return result;
        }
    }
}
