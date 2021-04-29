using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ApiClient.Http
{
    public abstract class ServiceBase
    {
        public Client Client { get; }

        public ServiceBase(Client restClient) =>
            Client = restClient ?? throw new NullReferenceException($"Client can't null");

        protected virtual async Task<HttpResponseMessage> SendRequestAsync(
            HttpRequestMessage request,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            CancellationToken cancellationToken = default)
        {
            return await Client.SendAsync(request, completionOption, cancellationToken);
        }

        protected virtual HttpRequestMessage CreateRequest(HttpMethod method, Uri endpoint) => 
            new HttpRequestMessage(method, endpoint);

        protected virtual JsonContent Json<T>(T payload, MediaTypeHeaderValue mediaTypeHeaderValue = null) =>
            JsonContent.Create(payload, mediaTypeHeaderValue, Client.JsonSerializerOptions);

        protected virtual async Task<T> ReadBodyAs<T>(
            HttpResponseMessage httpResponseMessage,
            CancellationToken cancellationToken = default) =>
            await httpResponseMessage.Content.ReadFromJsonAsync<T>(Client.JsonSerializerOptions, cancellationToken);

    }
}
