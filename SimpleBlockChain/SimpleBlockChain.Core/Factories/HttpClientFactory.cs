using System;
using System.Net.Http;

namespace SimpleBlockChain.Core.Factories
{
    public interface IHttpClientFactory
    {
        HttpClient BuildClient();
    }

    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient BuildClient()
        {
            return new HttpClient();
        }
    }
}
