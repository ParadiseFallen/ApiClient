using System;
using System.Net.Http;

namespace ApiClient.Extensions
{
    /// <summary>
    /// Fluent extensions for Http message building
    /// </summary>
    public static class HttpRequestMessageExtension
    {
        /// <summary>
        /// Set content to <c>HttpRequestMessage</c>
        /// </summary>
        /// <param name="content">Content</param>
        /// <returns><c>HttpRequestMessage</c> with new content</returns>
        public static HttpRequestMessage WithContent(this HttpRequestMessage message, HttpContent content)
        {
            message.Content = content;
            return message;
        }

        /// <summary>
        /// Configure <c>HttpRequestMessage</c>
        /// </summary>
        /// <param name="modification">Action for change <c>HttpRequestMessage</c></param>
        /// <returns>Configured <c>HttpRequestMessage</c></returns>
        public static HttpRequestMessage Configure(this HttpRequestMessage message, Action<HttpRequestMessage> modification = null)
        {
            modification?.Invoke(message);
            return message;
        }
    }
}
