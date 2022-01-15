using RatingBot.Bots.Telegram;
using RatingBot.Models.Db;
using RatingBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using VolguRatingBot.Services.Repository.Interface;

namespace RatingBot.Models.StateMachineActions
{
    public class StartRegistration : IAction
    {
        private readonly IRepository _repository;
        private readonly TelegramBotClient _client;

        public StartRegistration(IRepository repository, ITelegramBotGetter botGetter)
        {
            _repository = repository;
            _client = botGetter.GetBot().Result;
        }

        public virtual async Task ExecuteAsync(Update upd)
        {
            var chatId = upd.Message.Chat.Id;
            var student = new Student();

            student.ChatId = chatId;

            await _repository.AddAsync(student);
            await _client.SendTextMessageAsync(chatId,
                BotMessages.START_REGISTRATION,
                replyMarkup: ButtonsGetter.GetRegistrationButtons());
        }
    }
}
