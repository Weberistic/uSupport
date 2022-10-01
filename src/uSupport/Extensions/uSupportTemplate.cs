#if NETCOREAPP
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Razor;
#else
using System.Web;
using System.Web.Mvc;
#endif
using System;

namespace uSupport.Extensions
{
#if NETCOREAPP
    public abstract class uSupportTemplate<T> : RazorPage<T>
#else
    public abstract class uSupportTemplate<T> : WebViewPage<T>
#endif
    {
#if NETCOREAPP
        public string GetBaseUrl(HttpRequest request)
        {
            var uriString = request?.GetEncodedUrl() ?? null;
#else
        public string GetBaseUrl(HttpRequestBase request)
        {
            var uriString = request?.Url.AbsoluteUri ?? null;
#endif

            return !string.IsNullOrWhiteSpace(uriString) ? new Uri(uriString).GetLeftPart(UriPartial.Authority) : null;
        }
    }
}