using RatingBot.Services.Parser;
using Telegram.Bot.Types;
using VolguRatingBot.Services.Repository.Interface;

namespace RatingBot.Services
{
    public class UserDataCheker
    {
        private readonly IRepository _repository;
        private readonly ParserWorker _parserWorker;

        public UserDataCheker(IRepository repository, ParserWorker parserWorker)
        {
            _repository = repository;
            _parserWorker = parserWorker;
        }

        public async Task<bool> UserHasNoData(Update arg)
        {
            var chatId = arg.Message.Chat.Id;

            var student = await _repository.GetStudentAsync(chatId);

            if (student == null || student.Log == null || student.Pass == null || student.Semester == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UserHasData(Update arg)
        {
            var chatId = arg.Message.Chat.Id;

            var student = await _repository.GetStudentAsync(chatId);

            if (student == null || student.Log == null || student.Pass == null || student.Semester == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> UserIsRegistered(Update arg)
        {
            var chatId = arg.Message.Chat.Id;

            var student = await _repository.GetStudentAsync(chatId);

            var succesAuthorization = await _parserWorker
                .AuthorizationIsSuccess(student.Log, student.Pass);

            if (succesAuthorization == true && student.Semester != null && student.Semester <= 8 && student.Semester >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UserIsNotRegistered(Update arg)
        {
            var chatId = arg.Message.Chat.Id;

            var student = await _repository.GetStudentAsync(chatId);

            var succesAuthorization = await _parserWorker
                .AuthorizationIsSuccess(student.Log, student.Pass);

            if (succesAuthorization == true && student.Semester != null && student.Semester <= 8 && student.Semester >= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
