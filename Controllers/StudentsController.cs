using Microsoft.AspNetCore.Mvc;
using RatingBot.Bots.Telegram;
using RatingBot.Services;
using Telegram.Bot.Types;

namespace RatingBot.Controllers
{
    [ApiController]
    [Route("api/bot")]
    public class StudentsController : ControllerBase
    {
        private readonly ILogger<StudentsController> logger;
        private readonly DialogStateMachine machine;

        public StudentsController(ILogger<StudentsController> logger, DialogStateMachine machine)
        {
            this.logger = logger;
            this.machine = machine;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            if (update != null && update.Message!=null)
            {
                await machine.ExecuteCommand(update);
                return Ok();
            } 
            return BadRequest(ModelState);
        }
    }
}