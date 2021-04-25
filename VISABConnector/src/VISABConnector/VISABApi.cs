using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace VISABConnector
{
    /// <summary>
    /// Class for communicating with the VISAB api running in the java project
    /// </summary>
    public class VISABApi
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
        /// Relative endpoints for listing the currently active sessions in VISAB API
        /// </summary>
        private const string ENDPOINT_SESSION_LIST = "session/list";

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
        /// Event that is invoked before closing the session;
        /// </summary>
        public event EventHandler<ClosingEventArgs> CloseSessionEvent;

        /// <summary>
        /// The name of the game from which data will be sent
        /// </summary>
        public string Game { get; }

        /// <summary>
        /// Whether the session is active for the VISAB api
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// The RequestHandler used by the VISABApi object
        /// </summary>
        public IVISABRequestHandler RequestHandler { get; }

        /// <summary>
        /// The unique identifier for the current session
        /// </summary>
        public Guid SessionId { get; }

        /// <summary>
        /// Indicates if the VISAB api can receive data for the game
        /// </summary>
        /// <param name="game">The game to check</param>
        /// <returns>True if game is supported, false else</returns>
        public static async Task<bool> GameIsSupported(string game)
        {
            var handler = new VISABRequestHandler(null, Guid.Empty);
            var supportedGames = await handler.GetDeserializedResponseAsync<List<string>>(HttpMethod.Get, ENDPOINT_GAME_SUPPORTED, null, null).ConfigureAwait(false);

            return await Task.Run(() => supportedGames.Contains(game)).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a VISABApi object
        /// </summary>
        /// <param name="game">The game of which to sent data</param>
        /// <returns>A VISABApi object if the VISAB api is running, else null</returns>
        public static async Task<VISABApi> InitiateSession(string game)
        {
            var conn = new VISABApi(game, Guid.NewGuid());
            if (await GameIsSupported(game) && await conn.OpenSession())
            {
                conn.IsActive = true;
                return conn;
            }

            if (!await GameIsSupported(game))
                throw new Exception($"Game[{game}] is not supported by the VISAB Api!");

            return default;
        }

        /// <summary>
        /// Starts the VISAB jar
        /// </summary>
        /// <param name="pathToVisab">The path to the jar file</param>
        public static void StartVISAB(string pathToVisab)
        {
            // TODO: Start VISAB
        }

        /// <summary>
        /// Closes the session in the VISAB api
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CloseSession()
        {
            await Task.Run(() => CloseSessionEvent?.Invoke(this, new ClosingEventArgs { RequestHandler = RequestHandler })).ConfigureAwait(false);

            var closed = await RequestHandler.GetSuccessResponseAsync(HttpMethod.Get, ENDPOINT_SESSION_CLOSE, null, null).ConfigureAwait(false);

            if (closed)
                IsActive = false;

            return closed;
        }

        /// <summary>
        /// Indicates if the VISAB api (web api in java project) is running
        /// </summary>
        public async Task<bool> IsReachable()
        {
            return await RequestHandler.GetSuccessResponseAsync(HttpMethod.Get, ENDPOINT_PING_TEST, null, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a statistics object to the VISAB api
        /// </summary>
        /// <typeparam name="T">The type inheriting IVISABStatistics</typeparam>
        /// <param name="statistics">The statistics bject of type T</param>
        /// <returns></returns>
        public async Task<bool> SendStatistics<T>(T statistics) where T : IVISABStatistics
        {
            return await RequestHandler.GetSuccessResponseAsync(HttpMethod.Post, ENDPOINT_STATISTICS, null, statistics).ConfigureAwait(false);
        }

        /// <summary>
        /// Opens a session in the VISAB api
        /// </summary>
        /// <returns></returns>
        private async Task<bool> OpenSession()
        {
            return await RequestHandler.GetSuccessResponseAsync(HttpMethod.Get, ENDPOINT_SESSION_OPEN, null, null).ConfigureAwait(false);
        }
    }
}