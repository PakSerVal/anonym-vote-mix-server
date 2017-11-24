using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MixCustom.Models.Entities;
using MixCustom.Models;

namespace MixCustom.Controllers
{
    [Route("api")]
    public class ApiController: Controller
    {
        private Config config;
        public ApiController(Config config)
        {
            this.config = config;
        }

        [HttpPost("handle-bulletins")]
        public IActionResult ShuffleBulletins([FromBody] Bulletin[] bulletins)
        {
            if (bulletins.Length != 0 )
            {
                Bulletins bulletinsModel = new Bulletins(config);
                Bulletin[] shuffledBulletins =  bulletinsModel.Shuffle<Bulletin>(bulletins);
                bulletinsModel.sendToNextMix(shuffledBulletins);
                return Ok();
            }
            return BadRequest();
        }

    }
}
