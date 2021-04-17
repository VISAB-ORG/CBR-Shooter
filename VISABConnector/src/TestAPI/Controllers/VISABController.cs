using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TestAPI.Controllers
{
    [Route("api/VISAB")]
    [ApiController]
    public class VISABController : ControllerBase
    {
        [HttpGet("games")]
        public IActionResult GetSupportedGamesList()
        {
            var supportedGames = new List<string>
            {
                "TestGame"
            };

            return Ok(supportedGames);
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Ping received!");
        }

        [HttpPost("send/map")]
        public IActionResult ReceiveMap(object map)
        {
            return Ok("Map received!");
        }

        [HttpPost("send/statistics")]
        public IActionResult ReceiveStatistics(object statistics)
        {
            return Ok("Statistics received!");
        }
    }
}