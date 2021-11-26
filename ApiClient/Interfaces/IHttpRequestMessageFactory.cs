using ApiClient.Data.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient.Interfaces
{
    public interface IHttpRequestMessageFactory
    {

        public HttpRequestMessage Create(HttpMethod method, Uri endpoint);

        public HttpRequestMessage Create(HttpMethod method, string endpoint);

        public HttpRequestMessage Create(HttpEndpoint endpoint);
    }
}
