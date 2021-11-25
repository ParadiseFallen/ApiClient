using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace ApiClient.Http
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class Client : HttpClient
    {
        public JsonSerializerOptions JsonSerializerOptions { get; set; }

        public HttpClientHandler HttpHandler { get; }

        /// <summary>
        /// Accessor for <c>HttpHandler.CookieContainer</c>
        /// </summary>
        public CookieContainer Cookies
        {
            get => HttpHandler.CookieContainer;
            set => HttpHandler.CookieContainer = value;
        }

        public Client(
            HttpClientHandler handler = null, 
            JsonSerializerOptions serializerOptions = null,
            bool disposeHandler = true) : base(handler, disposeHandler)
        {
            HttpHandler = handler ?? new HttpClientHandler();
            JsonSerializerOptions = serializerOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                HttpHandler.Dispose();
            base.Dispose(disposing);
        }
    }
}
