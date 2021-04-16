using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VISABConnector
{
    public class Test
    {
        public static void Main(string[] args)
        {
            var testSerialization = new TestType();
            var json = JsonConvert.SerializeObject(
                testSerialization,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    ContractResolver = new IgnorePropertyContractResolver<DontSerialize>()
                });

            Console.WriteLine(json);
            var requestHandler = new VISABRequestHandler();

            requestHandler.GetJsonResponse(HttpMethod.Get, "ping", null, null);
        }

        public class TestType
        {
            public string ShouldAppear { get; set; } = "I deserve to be here";

            [DontSerialize]
            public string ShouldNotAppear { get; set; } = "I don't deserve to be here";
        }
    }
}
