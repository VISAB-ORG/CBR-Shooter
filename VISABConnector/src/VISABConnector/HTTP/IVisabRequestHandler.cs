using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace VISABConnector
{
    /// <summary>
    /// Used for making Requests to the VISAB API (web api in java project)
    /// </summary>
    public interface IVISABRequestHandler
    {
        /// <summary>
        /// Gets the deserialize response content
        /// </summary>
        /// <typeparam name="TResponse">The type to deserialize into</typeparam>
        /// <param name="httpMethod">The http method used</param>
        /// <param name="relativeUrl">The relative url to make the request to</param>
        /// <param name="queryParameters">The query parameters</param>
        /// <param name="body">The requests body</param>
        /// <returns>An awaitable Task whose body contains the deserialized object</returns>
        Task<TResponse> GetDeserializedResponseAsync<TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null, string body = null);

        /// <summary>
        /// Gets the deserialize response content
        /// </summary>
        /// <typeparam name="TResponse">The type to deserialize into</typeparam>
        /// <typeparam name="TBody">The type of the body</typeparam>
        /// <param name="httpMethod">The http method used</param>
        /// <param name="relativeUrl">The relative url to make the request to</param>
        /// <param name="queryParameters">The query parameters</param>
        /// <param name="body">The requests body</param>
        /// <returns>An awaitable Task whose body contains the deserialized object</returns>
        Task<TResponse> GetDeserializedResponseAsync<TBody, TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body);

        /// <summary>
        /// Gets the json response content
        /// </summary>
        /// <param name="httpMethod">The http method used</param>
        /// <param name="relativeUrl">The relative url to make the request to</param>
        /// <param name="queryParameters">The query parameters</param>
        /// <param name="body">The requests body</param>
        /// <returns>An awaitable Task whose body contains the json string</returns>
        Task<string> GetJsonResponseAsync(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body);

        /// <summary>
        /// Gets the json response content
        /// </summary>
        /// <typeparam name="TBody">The type of the body</typeparam>
        /// <param name="httpMethod">The http method used</param>
        /// <param name="relativeUrl">The relative url to make the request to</param>
        /// <param name="queryParameters">The query parameters</param>
        /// <param name="body">The requests body</param>
        /// <returns>An awaitable Task whose body contains the json string</returns>
        Task<string> GetJsonResponseAsync<TBody>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body);

        /// <summary>
        /// Gets whether the requests response status was successful
        /// </summary>
        /// <param name="httpMethod">The http method used</param>
        /// <param name="relativeUrl">The relative url to make the request to</param>
        /// <param name="queryParameters">The query parameters</param>
        /// <param name="body">The requests body</param>
        /// <returns>An awaitable Task whose body contains whether the request was successful</returns>
        Task<bool> GetSuccessResponseAsync(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body);

        /// <summary>
        /// Gets whether the requests response status was successful
        /// </summary>
        /// <typeparam name="TBody">The type of the body</typeparam>
        /// <param name="httpMethod">The http method used</param>
        /// <param name="relativeUrl">The relative url to make the request to</param>
        /// <param name="queryParameters">The query parameters</param>
        /// <param name="body">The requests body</param>
        /// <returns>An awaitable Task whose body contains whether the request was successful</returns>
        Task<bool> GetSuccessResponseAsync<TBody>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body);
    }
}