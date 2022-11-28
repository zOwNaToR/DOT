using Microsoft.AspNetCore.Http;

namespace Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetBaseUrl(this HttpRequest Request) => $"{Request.Scheme}://{Request.Host.Value}";
        
        public static string GetClientBaseUrl(this HttpRequest Request)
        {
            string baseUrl = Request.GetBaseUrl();
#if DEBUG
            baseUrl = "http://localhost:2998";
#endif
            return baseUrl;
        }
    }
}