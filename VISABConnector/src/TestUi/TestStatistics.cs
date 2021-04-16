using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VISABConnector;

namespace TestUi
{
    public class TestStatistics : IVISABStatistics
    {
        public string Game => "TestGame 123";

        public Guid SessionId { get; set; }

        public int Kills { get; set; }

        public string PlayerName { get; set; }
    }
}
