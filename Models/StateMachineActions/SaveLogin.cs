using RatingBot.Bots.Telegram;
using RatingBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using VolguRatingBot.Services.Repository.Interface;

namespace RatingBot.Models.StateMachineActions
{
    public class SaveLogin : IAction
    {
        private readonly IRepository _repository;
        private readonly TelegramBotClient _client;

        public SaveLogin(IRepository repository, ITelegramBotGetter botGetter)
        {
            _repository = repository;
            _client = botGetter.GetBot().Result;
        }

        public virtual async Task ExecuteAsync(Update upd)
        {
            var chatId = upd.Message.Chat.Id;
            var student = await _repository.GetStudentAsync(chatId);
            student.Log = upd.Message.Text;

            await _repository.UpdateAsync(student);

            if (student.Pass != null && student.Semester != null)
            {
                await _client.SendTextMessageAsync(chatId,
                       BotMessages.PASSWORD_SAVE,
                       replyMarkup: ButtonsGetter.GetChangeDataButtons());
            }
            else
            {
                await _client.SendTextMessageAsync(chatId,
                       BotMessages.LOGIN_SAVE,
                       replyMarkup: ButtonsGetter.GetRegistrationButtons());
            }
        }
    }
}
