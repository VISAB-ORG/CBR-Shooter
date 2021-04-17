using System;
using System.Collections.Generic;
using System.Net.Http;

namespace VISABConnector
{
    /// <summary>
    /// Class for communicating with the VISAB api running in the java project
    /// </summary>
    public class VISABApi : IDisposable
    {
        #region VISAB API relative endpoints

        /// <summary>
        /// Relative endpoint for checking useability for current game in VISAB API
        /// </summary>
        private const string ENDPOINT_GAME_SUPPORTED = "games";

        /// <summary>
        /// Relative endpoint for sending maps in VISAB API
        /// </summary>
        private const string ENDPOINT_MAP = "send/map";

        /// <summary>
        /// Relative endpoint for ping testing the VISAB API
        /// </summary>
        private const string ENDPOINT_PING_TEST = "ping";

        /// <summary>
        /// Relative endpoint for closing session in VISAB API
        /// </summary>
        private const string ENDPOINT_SESSION_CLOSE = "session/close";

        /// <summary>
        /// Relative endpoint for opening session in VISAB API
        /// </summary>
        private const string ENDPOINT_SESSION_OPEN = "session/open";

        /// <summary>
        /// Relative endpoints for checking session status in VISAB API
        /// </summary>
        private const string ENDPOINT_SESSION_STATUS = "session/status";

        /// <summary>
        /// Relative endpoint for sending statistics in VISAB API
        /// </summary>
        private const string ENDPOINT_STATISTICS = "send/statistics";

        #endregion VISAB API relative endpoints

        private VISABApi(string game, Guid sessionId)
        {
            Game = game;
            SessionId = sessionId;
            RequestHandler = new VISABRequestHandler(game, sessionId);
        }

        /// <summary>
        /// The name of the game from which data will be sent
        /// </summary>
        public string Game { get; }

        /// <summary>
        /// Indicates if the VISAB api (web api in java project) is running
        /// </summary>
        public bool IsReachable => RequestHandler.GetSuccessResponse(HttpMethod.Get, ENDPOINT_PING_TEST, null, null);

        /// <summary>
        /// The RequestHandler used by the VISABApi object
        /// </summary>
        public IVisabRequestHandler RequestHandler { get; }

        /// <summary>
        /// The unique identifier for the current session
        /// </summary>
        public Guid SessionId { get; }

        /// <summary>
        /// Creates a VISABApi object
        /// </summary>
        /// <param name="game">The game of which to sent data</param>
        /// <returns>A VISABApi object if the VISAB api is running, else null</returns>
        public static VISABApi InitiateSession(string game)
        {
            var conn = new VISABApi(game, Guid.NewGuid());
            if (conn.IsGameSupported(game) && conn.OpenSession())
                return conn;

            return default;
        }

        /// <summary>
        /// Close the session upon disposing
        /// </summary>
        public void Dispose() => CloseSession();

        /// <summary>
        /// Indicates if the VISAB api can receive data for the game
        /// </summary>
        /// <param name="game">The game to check</param>
        /// <returns>True if game is supported, false else</returns>
        public bool IsGameSupported(string game)
        {
            var supportedGames = RequestHandler.GetDeserializedResponse<List<string>>(HttpMethod.Get, ENDPOINT_GAME_SUPPORTED, null, null);

            return supportedGames.Contains(game);
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

        /// <summary>
        /// Closes the session in the VISAB api
        /// </summary>
        /// <returns></returns>
        private bool CloseSession()
        {
            return RequestHandler.GetSuccessResponse(HttpMethod.Get, ENDPOINT_SESSION_CLOSE, null, null);
        }

        /// <summary>
        /// Opens a session in the VISAB api
        /// </summary>
        /// <returns></returns>
        private bool OpenSession()
        {
            return RequestHandler.GetSuccessResponse(HttpMethod.Get, ENDPOINT_SESSION_OPEN, null, null);
        }
    }
}