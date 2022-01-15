using Microsoft.AspNetCore.Mvc;
using RatingBot.Services;
using Telegram.Bot.Types;

namespace RatingBot.Controllers
{
    [ApiController]
    [Route("api/bot")]
    public class DialogController : ControllerBase
    {
        private readonly ILogger<DialogController> logger;
        private readonly DialogStateMachine machine;

        public DialogController(DialogStateMachine machine)
        {
            this.logger = logger;
            this.machine = machine;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Update update)
        {
            if (update != null)
            {
                await machine.ExecuteCommandAsync(update);
            }

            return BadRequest(ModelState);
        }
    }
}