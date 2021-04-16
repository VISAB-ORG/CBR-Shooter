using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VISABConnector;

namespace TestAPI.Controllers
{
    [Route("api/VISAB")]
    [ApiController]
    public class VISABController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Ping received!");
        }

        [HttpPost("statistics")]
        public IActionResult ReceiveStatistics(object statistics)
        {
            return Ok("Statistics received!");
        }
        [HttpGet("useable")]
        public IActionResult GetCanReceive([FromQuery(Name = "game")] string game)
        {
            return Ok($"Ok, I can receive statistics for {game} now. If {game} is unity based, start by sending the map!");
        }

        [HttpPost("map")]
        public IActionResult ReceiveMap(object map)
        {
            return Ok("Map received!");
        }
    }
}
