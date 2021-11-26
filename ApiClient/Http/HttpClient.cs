using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace ApiClient.Http
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class HttpClient : System.Net.Http.HttpClient
    {
        public HttpClientHandler HttpHandler { get; }

        /// <summary>
        /// Accessor for <c>HttpHandler.CookieContainer</c>
        /// </summary>
        public CookieContainer Cookies
        {
            get => HttpHandler.CookieContainer;
            set => HttpHandler.CookieContainer = value;
        }

        public HttpClient(
            HttpClientHandler handler = null,
            JsonSerializerOptions serializerOptions = null,
            bool disposeHandler = true) : base(handler, disposeHandler)
        {
            HttpHandler = handler ?? new HttpClientHandler();
        }
    }
}
