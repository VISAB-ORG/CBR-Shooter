using System.Collections.Generic;
using System.Net.Http;

namespace VISABConnector
{
    public interface IRequestHandler
    {
        HttpResponseMessage GetResponse(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body);

        HttpResponseMessage GetResponse(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters);

        HttpResponseMessage GetResponse(HttpMethod httpMethod, string relativeUrl);
    }
}