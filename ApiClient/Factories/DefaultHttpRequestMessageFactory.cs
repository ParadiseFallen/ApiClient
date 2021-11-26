using ApiClient.Data.Records;
using ApiClient.Interfaces;
using System;
using System.Net.Http;

namespace ApiClient.Factories
{
    public sealed class DefaultHttpRequestMessageFactory : IHttpRequestMessageFactory
    {
        public HttpRequestMessage Create(HttpMethod method, Uri endpoint) => new (method, endpoint);

        public HttpRequestMessage Create(HttpMethod method, string endpoint) => new (method, endpoint);

        public HttpRequestMessage Create(HttpEndpoint endpoint) => new (endpoint.Method, endpoint.Uri);
    }
}
