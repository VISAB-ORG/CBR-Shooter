using System.Collections.Generic;
using System.Net.Http;

namespace VISABConnector
{
    public interface IVisabRequestHandler
    {
        public TResponse GetDeserializedResponse<TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body);

        public TResponse GetDeserializedResponse<TBody, TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body);

        public string GetJsonResponse(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body);

        public string GetJsonResponse<TBody>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body);

        public bool GetSuccessResponse(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body);

        public bool GetSuccessResponse<TBody>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body);
    }
}