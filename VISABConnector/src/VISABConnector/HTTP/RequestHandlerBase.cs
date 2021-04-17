using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace VISABConnector
{
    public class RequestHandlerBase
    {
        protected readonly HttpClient httpClient;

        public RequestHandlerBase(string baseAdress)
        {
            // Fix wrong baseAdress: https://stackoverflow.com/questions/23438416/why-is-httpclient-baseaddress-not-working
            var _baseAdress = baseAdress.EndsWith('/') ? baseAdress : baseAdress + '/';

            httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_baseAdress)
            };

            // Set the default content header media type
            // httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Default.MediaType));
        }

        public HttpResponseMessage GetResponse(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null, string body = null)
        {
            return httpClient.Send(PrepareRequest(httpMethod, relativeUrl, queryParameters, body));
        }

        protected static string BuildParameterizedUrl(string relativeUrl, IEnumerable<string> queryParameters)
        {
            return $"{relativeUrl}?" + string.Join('&', queryParameters);
        }

        protected static HttpRequestMessage PrepareRequest(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null, string body = null)
        {
            // Fix wrong relativeUrl: https://stackoverflow.com/questions/23438416/why-is-httpclient-baseaddress-not-working
            var url = relativeUrl.StartsWith('/') ? relativeUrl.Remove(0, 1) : relativeUrl;

            if (queryParameters != null)
                url = BuildParameterizedUrl(url, queryParameters);

            var request = new HttpRequestMessage(httpMethod, url);

            if (!string.IsNullOrWhiteSpace(body))
                request.Content = new StringContent(body, Default.Encoding, Default.ContentMediaType);

            return request;
        }
    }
}