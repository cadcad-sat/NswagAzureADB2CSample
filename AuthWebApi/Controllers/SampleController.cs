using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuthWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SampleController : ControllerBase
    {
        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILogger<SampleController> logger;
        public SampleController(ILogger<SampleController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("Get")]
        public async Task<ActionResult> Get()
        {
            await Task.Delay(0);
            var id = Guid.NewGuid();
            logger.LogDebug(id.ToString());
            return Ok(id);
        }
    }
}
