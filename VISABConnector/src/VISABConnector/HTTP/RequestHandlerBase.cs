using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VISABConnector
{
    /// <summary>
    /// Base class used for making HTTP requests. Classes making HTTP Requests should inherit from this.
    /// </summary>
    public class RequestHandlerBase
    {
        /// <summary>
        /// The used HttpClient
        /// </summary>
        protected readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a RequestHandlerBase instance
        /// </summary>
        /// <param name="baseAdress">The base adress for the HttpClient</param>
        public RequestHandlerBase(string baseAdress)
        {
            // Fix wrong baseAdress: https://stackoverflow.com/questions/23438416/why-is-httpclient-baseaddress-not-working
            var _baseAdress = baseAdress.EndsWith("/") ? baseAdress : baseAdress + '/';

            httpClient = new HttpClient { BaseAddress = new System.Uri(_baseAdress) };

            // Set the default content header media type
            // httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Default.MediaType));
        }

        /// <summary>
        /// Makes a http request and gets the HttpResponseMessage object
        /// </summary>
        /// <param name="httpMethod">The http method used</param>
        /// <param name="relativeUrl">The relative url to make the request to</param>
        /// <param name="queryParameters">The query parameters</param>
        /// <param name="body">The requests body</param>
        /// <returns>The HttpResponseMessage object</returns>
        public async Task<HttpResponseMessage> GetResponseAsync(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null, string body = null)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(Default.REQUEST_TIMEOUT));

            var request = PrepareRequest(httpMethod, relativeUrl, queryParameters, body);
            try
            {
                return await httpClient.SendAsync(request, cts.Token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return new HttpResponseMessage
                {
                    RequestMessage = request,
                    ReasonPhrase = $"[VISABConnector] Failed to make request to VISAB api.",
                    StatusCode = System.Net.HttpStatusCode.BadGateway
                };
            }
        }

        /// <summary>
        /// Builds a parametrized url from the given query parameters
        /// </summary>
        /// <param name="relativeUrl">The relative url without the parameters</param>
        /// <param name="queryParameters">The query parameters to add</param>
        /// <returns>A parametrized url</returns>
        protected static string BuildParameterizedUrl(string relativeUrl, IEnumerable<string> queryParameters)
        {
            return $"{relativeUrl}?" + string.Join("&", queryParameters);
        }

        /// <summary>
        /// Builds a HttpRequestMessage object
        /// </summary>
        /// <param name="httpMethod">The http method used</param>
        /// <param name="relativeUrl">The relative url to make the request to</param>
        /// <param name="queryParameters">The query parameters</param>
        /// <param name="body">The requests body</param>
        /// <returns>The built HttpRequestMessage object</returns>
        protected static HttpRequestMessage PrepareRequest(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null, string body = null)
        {
            // Fix wrong relativeUrl: https://stackoverflow.com/questions/23438416/why-is-httpclient-baseaddress-not-working
            var url = relativeUrl.StartsWith("/") ? relativeUrl.Remove(0, 1) : relativeUrl;

            if (queryParameters != null)
                url = BuildParameterizedUrl(url, queryParameters);

            var request = new HttpRequestMessage(httpMethod, url);

            if (!string.IsNullOrWhiteSpace(body))
                request.Content = new StringContent(body, Default.Encoding, Default.ContentMediaType);

            return request;
        }

        /// <summary>
        /// Reads the content from a HttpResponseMessage
        /// </summary>
        /// <param name="httpResponse">The HttpResponseMessage to read the content from</param>
        /// <returns>The content as a string, empty string if request wasn't successful</returns>
        protected static async Task<string> GetResponseContentAsync(HttpResponseMessage httpResponse)
        {
            if (httpResponse.IsSuccessStatusCode)
                return await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            else
                return "";
        }
    }
}