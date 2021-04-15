namespace VISABConnector
{
    public class VISABConnection
    {
        static VISABConnection()
        {
        }

        public IRequestHandler RequestHandler { get; }

        public static VISABConnection Connect()
        {
            // Bla bla bla connect to visab, falls positive rueckmeldung => Geb connection object
            // zurueck andernfalls mal gucken

            return new VISABConnection();
        }

        public bool SendStatistics<T>(T statistics) where T : IVISABStatistics
        {
            // Send the statistics

            return default;
        }
    }
}