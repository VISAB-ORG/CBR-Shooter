using System.Collections.Generic;
using System.Net.Http;

namespace VISABConnector
{
    /// <summary>
    /// Used for making Requests to the VISAB API (web api in java project)
    /// </summary>
    public interface IVISABRequestHandler
    {
        TResponse GetDeserializedResponse<TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body);

        TResponse GetDeserializedResponse<TBody, TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body);

        string GetJsonResponse(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body);

        string GetJsonResponse<TBody>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body);

        bool GetSuccessResponse(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body);

        bool GetSuccessResponse<TBody>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body);
    }
}