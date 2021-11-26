using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApiClient.Interfaces
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
