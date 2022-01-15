using Telegram.Bot.Types;

namespace RatingBot.Models.StateMachineActions
{
    public interface IAction
    {
        public Task ExecuteAsync(Update upd);
    }
}
