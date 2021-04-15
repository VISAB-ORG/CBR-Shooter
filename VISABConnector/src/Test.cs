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
            var connection = VISABConnection.Connect();
            var client = new HttpClient();
        }
    }
}
