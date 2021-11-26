using System.Net.Http;

namespace ApiClient.Data.Records
{
    /// <summary>
    /// Describes Http endpoint
    /// </summary>
    public record HttpEndpoint
    {
        public HttpEndpoint(
            string uri = null,
            HttpMethod method = null,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead) =>
            (Uri, Method, CompletionOption) = (uri, method, completionOption);

        public string Uri { get; init; } = default;

        public HttpMethod Method { get; init; } = null;

        public HttpCompletionOption CompletionOption { get; init; } = HttpCompletionOption.ResponseContentRead;
    }
}
