using RatingBot.Bots.Telegram;
using RatingBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using VolguRatingBot.Services.Repository.Interface;

namespace RatingBot.Models.StateMachineActions
{
    public class SaveSemester : IAction
    {
        private readonly IRepository _repository;
        private readonly TelegramBotClient _client;

        public SaveSemester(IRepository repository, ITelegramBotGetter botGetter)
        {
            _repository = repository;
            _client = botGetter.GetBot().Result;
        }

        public virtual async Task ExecuteAsync(Update upd)
        {
            var chatId = upd.Message.Chat.Id;
            var student = await _repository.GetStudentAsync(chatId);
            student.Semester = int.Parse(upd.Message.Text);

            await _repository.UpdateAsync(student);

            if (student.Semester != null && student.Log != null && student.Pass != null)
            {
                await _client.SendTextMessageAsync(chatId,
                       BotMessages.PASSWORD_SAVE,
                       replyMarkup: ButtonsGetter.GetChangeDataButtons());
            }
            else
            {
                await _client.SendTextMessageAsync(chatId,
                       BotMessages.SEMESTER_SAVE,
                       replyMarkup: ButtonsGetter.GetRegistrationButtons());
            }
        }
    }
}
