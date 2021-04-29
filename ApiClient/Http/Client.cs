using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace ApiClient.Http
{
    public class Client : HttpClient
    {
        public JsonSerializerOptions JsonSerializerOptions { get; set; }
        public HttpClientHandler HttpHandler { get; }

        /// <summary>
        /// Accessor for <c>HttpHandler.CookieContainer</c>
        /// </summary>
        public CookieContainer Cookies { get => HttpHandler.CookieContainer; set => HttpHandler.CookieContainer = value; }

        public Client(HttpClientHandler handler, JsonSerializerOptions serializerOptions = null) : base(handler, true)
        {
            HttpHandler = handler ?? throw new NullReferenceException("HttpClientHandler can't be null.");
            JsonSerializerOptions = serializerOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                HttpHandler.Dispose();
            base.Dispose(disposing);
        }

        public static Client Create(HttpClientHandler handler = null, JsonSerializerOptions serializerOptions = null) =>
            new Client(handler ?? new HttpClientHandler(), serializerOptions);
    }
}
