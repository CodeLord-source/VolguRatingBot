using RatingBot.Bots.Telegram;
using RatingBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RatingBot.Models.StateMachineActions
{
    public class ChangeData : IAction
    {
        private readonly TelegramBotClient _client;

        public ChangeData(ITelegramBotGetter getter)
        {
            _client = getter.GetBot().Result;
        }

        public virtual async Task ExecuteAsync(Update upd)
        {
            var chatId = upd.Message.Chat.Id;

            await _client.SendTextMessageAsync(chatId,
                   BotMessages.DATA_CHANGE,
                   replyMarkup: ButtonsGetter.GetChangeDataButtons());
        }
    }
}
