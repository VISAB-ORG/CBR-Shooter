using System.Text;

namespace VISABConnector
{
    /// <summary>
    /// Contains default values used for communication with the VISAB api in the java project
    /// </summary>
    public static class Default
    {
        public static readonly Encoding Encoding = Encoding.UTF8;

        public static readonly string ContentMediaType = "application/json";

        public static readonly string VISABBaseAdress = $"https://localhost:{PORT}";

        public const int PORT = 2673;
    }
}