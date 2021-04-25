using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace TestAPI.Controllers
{
    [Route("api/session")]
    [ApiController]
    public class SessionController : Controller
    {
        public static IDictionary<Guid, string> ActiveSessions { get; } = new Dictionary<Guid, string>();

        [HttpGet("close")]
        public IActionResult CloseSession()
        {
            var headers = Request.Headers;

            if (headers.ContainsKey("sessionId"))
            {
                Guid.TryParse(headers["sessionId"], out var sessionId);
                ActiveSessions.Remove(sessionId);

                return Ok("Removed session");
            }
            return Ok("Session was not active.");

            // We should likely also return true if no session was open right?
            return BadRequest("Session with sessionId was not open.");
        }

        [HttpGet("list")]
        public IActionResult GetCurrentlyActive()
        {
            return Ok(ActiveSessions);
        }

        [HttpGet("status")]
        public IActionResult GetStatus([FromQuery] Guid sessionId)
        {
            if (ActiveSessions.ContainsKey(sessionId))
                return Ok($"Session is currently active with game {ActiveSessions[sessionId]}.");
            else
                return Ok($"Session is not currently active.");
        }

        [HttpGet("open")]
        public IActionResult OpenSession()
        {
            var headers = Request.Headers;

            var sessionId = Guid.Empty;
            var game = "";

            if (headers.ContainsKey("sessionId"))
                Guid.TryParse(headers["sessionId"], out sessionId);

            if (headers.ContainsKey("game"))
                game = headers["game"];

            if (sessionId != default && game != default)
            {
                ActiveSessions.Add(sessionId, game);

                return Ok("Session added.");
            }

            return BadRequest("sessionId or game not found in headers.");
        }
    }
}