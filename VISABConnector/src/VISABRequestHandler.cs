﻿using Newtonsoft.Json; // TODO: Potentially just use gson, as is used in the VISAB java project
using System.Collections.Generic;
using System.Net.Http;

namespace VISABConnector
{
    public class VISABRequestHandler : RequestHandlerBase, IVisabRequestHandler
    {
        public VISABRequestHandler() : base(Default.BaseAdress)
        {
        }

        public TResponse GetDeserializedResponse<TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null, string body = null)
        {
            var jsonResponse = GetJsonResponse(httpMethod, relativeUrl, queryParameters, body);

            return JsonConvert.DeserializeObject<TResponse>(jsonResponse);
        }

        public TResponse GetDeserializedResponse<TBody, TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body)
        {
            var jsonResponse = GetJsonResponse(httpMethod, relativeUrl, queryParameters, body);

            return JsonConvert.DeserializeObject<TResponse>(jsonResponse);
        }

        public string GetJsonResponse(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body)
        {
            return GetResponse(httpMethod, relativeUrl, queryParameters, body).Content.ReadAsStringAsync().Result;
        }

        public string GetJsonResponse<TBody>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body)
        {
            var json = JsonConvert.SerializeObject(body, Formatting.Indented);

            return GetJsonResponse(httpMethod, relativeUrl, queryParameters, json);
        }

        public bool GetSuccessResponse(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, string body)
        {
            return GetResponse(httpMethod, relativeUrl, queryParameters, body).IsSuccessStatusCode;
        }

        public bool GetSuccessResponse<TBody>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TBody body)
        {
            var json = JsonConvert.SerializeObject(body, Formatting.Indented);

            return GetSuccessResponse(httpMethod, relativeUrl, queryParameters, json);
        }
    }
}