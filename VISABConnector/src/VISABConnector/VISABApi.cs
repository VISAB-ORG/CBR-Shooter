using System.Net.Http;

namespace VISABConnector
{
    /// <summary>
    /// Class for communicating with the VISAB api running in the java project
    /// </summary>
    public class VISABApi
    {
        #region VISAB API relative endpoints

        /// <summary>
        /// Relative endpoint for sending maps in VISAB API
        /// </summary>
        private const string ENDPOINT_MAP = "map";

        /// <summary>
        /// Relative endpoint for ping testing the VISAB API
        /// </summary>
        private const string ENDPOINT_PING_TEST = "ping";

        /// <summary>
        /// Relative endpoint for sending statistics in VISAB API
        /// </summary>
        private const string ENDPOINT_STATISTICS = "statistics";

        /// <summary>
        /// Relative endpoint for checking useability for current game in VISAB API
        /// </summary>
        private const string ENDPOINT_USEABLE_FOR_GAME = "useable";

        #endregion VISAB API relative endpoints

        private VISABApi(string game)
        {
            Game = game;
            RequestHandler = new VISABRequestHandler();
        }

        /// <summary>
        /// The name of the game from which data will be sent
        /// </summary>
        public string Game { get; }

        /// <summary>
        /// Indicates if the VISAB api (web api in java project) is running
        /// </summary>
        public bool IsRunning => RequestHandler.GetSuccessResponse(HttpMethod.Get, ENDPOINT_PING_TEST, null, null);

        /// <summary>
        /// Indicates if the VISAB api can currently receive data for the game
        /// </summary>
        public bool IsUseable
        {
            get
            {
                return RequestHandler.GetSuccessResponse(
                    HttpMethod.Get,
                    ENDPOINT_USEABLE_FOR_GAME,
                    new string[] { $"game={Game}" },
                    null);
            }
        }

        /// <summary>
        /// The RequestHandler used by the VISABApi object
        /// </summary>
        public IVisabRequestHandler RequestHandler { get; }

        /// <summary>
        /// Creates a VISABApi object
        /// </summary>
        /// <param name="game">The game of which to sent data</param>
        /// <returns>A VISABApi object if the VISAB api is running, else null</returns>
        public static VISABApi Initiate(string game)
        {
            var conn = new VISABApi(game);
            if (conn.IsUseable)
                return conn;

            return default;
        }

        /// <summary>
        /// Sends a statistics object to the VISAB api
        /// </summary>
        /// <typeparam name="T">The type inheriting IVISABStatistics</typeparam>
        /// <param name="statistics">The statistics bject of type T</param>
        /// <returns></returns>
        public bool SendStatistics<T>(T statistics) where T : IVISABStatistics
        {
            return RequestHandler.GetSuccessResponse(HttpMethod.Post, ENDPOINT_STATISTICS, null, statistics);
        }
    }
}