using Newtonsoft.Json.Linq;
using System;

namespace EhealthBlockChain.API.Extensions
{
    public static class JObjectExtensions
    {
        public static string TryGetString(this JObject jObj, string name)
        {
            if (jObj == null)
            {
                throw new ArgumentNullException(nameof(jObj));
            }

            JToken jToken;
            if (!jObj.TryGetValue(name, out jToken))
            {
                return null;
            }

            return jToken.ToString();
        }
    }
}
