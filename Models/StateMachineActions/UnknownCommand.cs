using RatingBot.Bots.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RatingBot.Models.StateMachineActions
{
    public class UnknownCommand : IAction
    {
        private readonly TelegramBotClient _client;

        public UnknownCommand(ITelegramBotGetter botGetter)
        {
            _client = botGetter.GetBot().Result;
        }

        public virtual async Task ExecuteAsync(Update upd)
        {
            var chatId = upd.Message.Chat.Id;

            await _client.SendTextMessageAsync(chatId,
                BotMessages.UNKNOWN_COMMAND);
        }
    }
}
