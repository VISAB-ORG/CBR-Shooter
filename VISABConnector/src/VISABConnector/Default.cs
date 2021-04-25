using System.Text;

namespace VISABConnector
{
    /// <summary>
    /// Contains default values used for communication with the VISAB api in the java project
    /// </summary>
    public static class Default
    {
        /// <summary>
        /// The port that VISAB is running on
        /// </summary>
        public const int PORT = 2673;

        /// <summary>
        /// Timeout for HTTP Requests in seconds
        /// </summary>
        public const int REQUEST_TIMEOUT = 1;

        public static readonly string ContentMediaType = "application/json";
        public static readonly Encoding Encoding = Encoding.UTF8;
        public static readonly string VISABBaseAdress = $"http://localhost:{PORT}";
    }
}