using RatingBot.Bots.Telegram;
using RatingBot.Services;
using RatingBot.Services.Parser;
using Telegram.Bot;
using Telegram.Bot.Types;
using VolguRatingBot.Services.Repository.Interface;

namespace RatingBot.Models.StateMachineActions
{
    public class SaveChanges : IAction
    {
        private readonly IRepository _repository;
        private readonly TelegramBotClient _client;

        public SaveChanges(IRepository repository, ITelegramBotGetter botGetter)
        {
            _repository = repository;
            _client = botGetter.GetBot().Result;
        }

        public virtual async Task ExecuteAsync(Update upd)
        {
            var chatid = upd.Message.Chat.Id;
            var student = await _repository.GetStudentAsync(chatid);

            await _client.SendTextMessageAsync(chatid,
                      BotMessages.CHANGES_SAVED,
                      replyMarkup: ButtonsGetter.GetRegisteredButtons());
        }
    }
}
