using ApiClient.Data.Records;
using ApiClient.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ApiClient.Http
{
    public abstract class HttpServiceBase
    {
        #region Properties

        public JsonSerializerOptions JsonSerializerOptions { get; set; }

        public HttpClient HttpClient { get; }

        public IHttpRequestMessageFactory RequestMessageFactory { get; }

        #endregion

        public HttpServiceBase(IHttpRequestMessageFactory requestMessageFactory, HttpClient httpClient)
        {

            HttpClient = httpClient ?? throw new ArgumentNullException($"Client can't null");
            JsonSerializerOptions ??= new JsonSerializerOptions(JsonSerializerDefaults.Web);
            RequestMessageFactory = requestMessageFactory;
        }

        protected virtual Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            CancellationToken cancellationToken = default) =>
                HttpClient.SendAsync(request, completionOption, cancellationToken);

        protected virtual HttpResponseMessage Send(
             HttpRequestMessage request,
             HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
             CancellationToken cancellationToken = default) =>
                HttpClient.Send(request, completionOption, cancellationToken);

        #region CreateRequest()

        protected HttpRequestMessage CreateRequestMessage(HttpMethod method, Uri endpoint) =>
            RequestMessageFactory.Create(method, endpoint);

        protected HttpRequestMessage CreateRequestMessage(HttpMethod method, string endpoint) =>
            RequestMessageFactory.Create(method, endpoint);

        protected HttpRequestMessage CreateRequestMessage(HttpEndpoint endpoint) =>
            RequestMessageFactory.Create(endpoint);

        #endregion

        #region Helpers

        protected virtual JsonContent Json<T>(T payload, MediaTypeHeaderValue mediaTypeHeaderValue = null) =>
            JsonContent.Create(payload, mediaTypeHeaderValue, JsonSerializerOptions);

        protected virtual async Task<T> ReadJsonBodyAs<T>(
            HttpResponseMessage httpResponseMessage,
            CancellationToken cancellationToken = default) =>
                await httpResponseMessage
                    .Content
                    .ReadFromJsonAsync<T>(JsonSerializerOptions, cancellationToken);
        #endregion
    }
}
