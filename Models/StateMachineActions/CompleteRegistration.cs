using RatingBot.Bots.Telegram;
using RatingBot.Services;
using RatingBot.Services.Parser;
using Telegram.Bot;
using Telegram.Bot.Types;
using VolguRatingBot.Services.Repository.Interface;

namespace RatingBot.Models.StateMachineActions
{
    public class CompleteRegistration : IAction
    {
        private readonly TelegramBotClient _client;

        public CompleteRegistration(IRepository repository, ITelegramBotGetter botGetter)
        {
            _client = botGetter.GetBot().Result;
        }

        public virtual async Task ExecuteAsync(Update upd)
        {
            var chatid = upd.Message.Chat.Id;

            await _client.SendTextMessageAsync(chatid,
                      BotMessages.COMPLETE_REGISTRATION,
                      replyMarkup: ButtonsGetter.GetRegisteredButtons());
        }
    }
}
