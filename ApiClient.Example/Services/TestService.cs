using ApiClient.Example.Interfaces;
using ApiClient.Http;
using ApiClient.Interfaces;

namespace ApiClient.Example.Services
{
    internal class TestService : HttpServiceBase, ITest
    {
        public TestService(IHttpRequestMessageFactory messageBuilder, Http.HttpClient httpClient) : base(messageBuilder, httpClient)
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
