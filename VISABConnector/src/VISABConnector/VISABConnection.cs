namespace VISABConnector
{
    public class VISABConnection
    {
        static VISABConnection()
        {
        }

        public static VISABConnection Connect()
        {
            return new VISABConnection();
        }

        public bool SendStatistics<T>(T statistics) where T : IVISABStatistics
        {
            return default;
        }
    }
}