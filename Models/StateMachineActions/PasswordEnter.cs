using RatingBot.Bots.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RatingBot.Models.StateMachineActions
{
    public class PasswordEnter : IAction
    {
        private readonly TelegramBotClient _client;

        public PasswordEnter(ITelegramBotGetter getter)
        {
            _client = getter.GetBot().Result;
        }

        public virtual async Task ExecuteAsync(Update upd)
        {
            var chatId = upd.Message.Chat.Id;

            await _client.SendTextMessageAsync(chatId,
                    BotMessages.PASSWORD_ENTER,
                    replyMarkup: new ReplyKeyboardRemove());
        }
    }
}
