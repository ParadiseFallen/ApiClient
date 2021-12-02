using ApiClient.Interfaces.Http;

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ApiClient.Http
{
    public class DefaultHttpClient : IHttpMessageInvoker
    {
        public HttpClient HttpClient { get; set; }

        public DefaultHttpClient(HttpClient httpClient) => HttpClient = httpClient;

        public HttpResponseMessage Send(
            HttpRequestMessage request,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            CancellationToken cancellationToken = default) =>
            HttpClient.Send(request, completionOption, cancellationToken);


        public Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            CancellationToken cancellationToken = default) =>
            HttpClient.SendAsync(request, completionOption, cancellationToken);
    }
}
