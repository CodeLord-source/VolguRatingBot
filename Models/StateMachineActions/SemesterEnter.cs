using RatingBot.Bots.Telegram;
using RatingBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RatingBot.Models.StateMachineActions
{
    public class SemesterEnter : IAction
    {
        private readonly TelegramBotClient _client;

        public SemesterEnter(ITelegramBotGetter getter)
        {
            _client = getter.GetBot().Result;
        }

        public virtual async Task ExecuteAsync(Update upd)
        {
            var chatId = upd.Message.Chat.Id;

            await _client.SendTextMessageAsync(chatId,
                    BotMessages.SEMESTER_ENTER,
                    replyMarkup: ButtonsGetter.GetSemesterButtons());
        }
    }
}
