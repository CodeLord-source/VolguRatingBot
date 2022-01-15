using RatingBot.Bots.Telegram;
using RatingBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RatingBot.Models.StateMachineActions
{
    public class SendStartMessage : IAction
    {
        private readonly TelegramBotClient _client;

        public SendStartMessage(ITelegramBotGetter botGetter)
        {
            _client = botGetter.GetBot().Result;
        }

        public virtual async Task ExecuteAsync(Update upd)
        {
            var chatId = upd.Message.Chat.Id;

            await _client.SendTextMessageAsync(chatId,
                BotMessages.START,
                replyMarkup: ButtonsGetter.GetRegistrationButtons());
        }
    }
}
