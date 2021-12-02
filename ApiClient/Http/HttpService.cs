using ApiClient.Data.Records;
using ApiClient.Interfaces.Http;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ApiClient.Http
{
    public abstract class HttpService<THttpClient> where THttpClient : IHttpMessageInvoker
    {
        #region Properties

        public JsonSerializerOptions JsonSerializerOptions { get; set; }

        public THttpClient Http { get; }

        #endregion

        public HttpService(THttpClient http, JsonSerializerOptions jsonSerializerOptions = null)
        {
            Http = http ?? throw new ArgumentNullException(nameof(THttpClient));
            JsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);
        }

        #region Send
        protected async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            CancellationToken cancellationToken = default) =>
                await Http.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false);

        protected HttpResponseMessage Send(
             HttpRequestMessage request,
             HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
             CancellationToken cancellationToken = default) => Http.Send(request, completionOption, cancellationToken);
        #endregion

        #region CreateRequest()

        /// <summary>
        /// Create <c>HttpRequestMessage</c> from given <c>HttpMethod</c> and Uri
        /// </summary>
        /// <param name="method">Method</param>
        /// <param name="endpoint">uri</param>
        /// <returns><c>HttpRequestMessage</c></returns>
        protected static HttpRequestMessage CreateRequestMessage(HttpMethod method, Uri endpoint) =>
            new(method, endpoint);

        /// <summary>
        /// Create <c>HttpRequestMessage</c> from given <c>HttpMethod</c> and Uri
        /// </summary>
        /// <param name="method">Method</param>
        /// <param name="endpoint">uri</param>
        /// <returns><c>HttpRequestMessage</c></returns>
        protected static HttpRequestMessage CreateRequestMessage(HttpMethod method, string endpoint) =>
            new(method, endpoint);

        /// <summary>
        /// Create <c>HttpRequestMessage</c> from given <c>HttpEndpoint</c>
        /// Uses <c>endpoint.Method</c> and <c>endpoint.Uri</c>
        /// </summary>
        /// <param name="endpoint"><c>HttpEndpoint</c></param>
        /// <returns><c>HttpRequestMessage</c></returns>
        protected static HttpRequestMessage CreateRequestMessage(HttpEndpoint endpoint) =>
            new(endpoint.Method, endpoint.Uri);

        #endregion

        #region Helpers

        protected virtual JsonContent Json<T>(T payload, MediaTypeHeaderValue mediaTypeHeaderValue = null) =>
            JsonContent.Create(payload, mediaTypeHeaderValue, JsonSerializerOptions);

        protected virtual async Task<T> ReadJsonContentAs<T>(
            HttpResponseMessage httpResponseMessage,
            CancellationToken cancellationToken = default) =>
                await httpResponseMessage
                    .Content
                    .ReadFromJsonAsync<T>(JsonSerializerOptions, cancellationToken);
        #endregion
    }

    public abstract class HttpService : HttpService<DefaultHttpClient>
    {
        public HttpService(DefaultHttpClient http, JsonSerializerOptions jsonSerializerOptions = null) : base(http, jsonSerializerOptions)
        {
        }
    }
}
