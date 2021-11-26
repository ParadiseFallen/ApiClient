using ApiClient.Example.Interfaces;
using ApiClient.Http;
using ApiClient.Interfaces;

namespace ApiClient.Example.Services
{
    internal class TestService : HttpService, ITest
    {
        public TestService(IHttpRequestMessageFactory messageBuilder, CustomHttpClient httpClient) : base(messageBuilder, httpClient)
        {

        }

        public async Task<bool> Foo()
        {
            var request = RequestMessageFactory.Create(HttpMethod.Get, "");
            var response = await SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
