using RatingBot.Bots.Telegram;
using RatingBot.Services.Parser;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using VolguRatingBot.Services.Repository.Interface;

namespace RatingBot.Models.StateMachineActions
{
    public class GetRating : IAction
    {
        private readonly IRepository _repository;
        private readonly TelegramBotClient _client;
        private readonly ParserWorker _parser;

        public GetRating(IRepository repository, ITelegramBotGetter botGetter, ParserWorker parser)
        {
            _repository = repository;
            _parser = parser;
            _client = botGetter.GetBot().Result;
        }

        public virtual async Task ExecuteAsync(Update upd)
        {
            var chatId = upd.Message.Chat.Id;
            var student = await _repository.GetStudentAsync(chatId);
            var message = await _parser.GetDataMessage(student.Log, student.Pass, (int)student.Semester);

            await _client.SendTextMessageAsync(chatId,
                $"{message}",
                ParseMode.Html);
        }
    }
}
