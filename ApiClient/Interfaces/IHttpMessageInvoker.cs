using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ApiClient.Interfaces.Http
{
    public interface IHttpMessageInvoker
    {
        public Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            CancellationToken cancellationToken = default);

        public HttpResponseMessage Send(
             HttpRequestMessage request,
             HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
             CancellationToken cancellationToken = default);
    }
}
