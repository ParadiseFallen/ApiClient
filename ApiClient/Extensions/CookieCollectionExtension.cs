using System;
using System.Net;

namespace ApiClient.Extensions
{
    public static class CookieCollectionExtension
    {
        /// <summary>
        /// Try to get cookie value from <c>CookieContainer</c>
        /// </summary>
        /// <param name="uri">Cookies uri</param>
        /// <param name="cookieName">Cookie name</param>
        /// <param name="cookie">Cookie value</param>
        /// <returns>True if cookie exsist</returns>
        public static bool TryGetCookie(
            this CookieContainer cookies,
            Uri uri,
            string cookieName,
            out Cookie cookie)
        {
            cookie = cookies.GetCookies(uri)[cookieName];
            return cookie is not null;
        }

    }
}
