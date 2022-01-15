using RatingBot.Bots.Telegram;
using RatingBot.Services.Parser;
using VolguRatingBot.Services.Repository.Interface;

namespace RatingBot.Models.StateMachineActions
{
    public class ActionsInitializer
    {
        private readonly IRepository _repository;
        private readonly ParserWorker _parserWorker;
        private readonly ITelegramBotGetter _botGetter;

        public ActionsInitializer(IRepository repository, ParserWorker parserWorker, ITelegramBotGetter botGetter)
        {
            _repository = repository;
            _parserWorker = parserWorker;
            _botGetter = botGetter;
        }

        public virtual Dictionary<string, IAction> Initialize()
        {
            var actions = new Dictionary<string, IAction>()
            {
                {ActionsNames.ChangeData,new ChangeData(_botGetter) },
                {ActionsNames.CompleteRegistration,new CompleteRegistration(_repository,_botGetter) },
                {ActionsNames.DataSaveError,new DataSaveError(_botGetter) },
                {ActionsNames.GetRating,new GetRating(_repository,_botGetter,_parserWorker) },
                {ActionsNames.LoginEnter,new LoginEnter(_botGetter) },
                {ActionsNames.PasswordEnter,new PasswordEnter(_botGetter) },
                {ActionsNames.SemesterEnter,new SemesterEnter(_botGetter) },
                {ActionsNames.SaveChanges,new SaveChanges(_repository,_botGetter) },
                {ActionsNames.SaveLogin,new SaveLogin(_repository,_botGetter) },
                {ActionsNames.SavePassword,new SavePassword(_repository,_botGetter) },
                {ActionsNames.SaveSemester,new SaveSemester(_repository,_botGetter) },
                {ActionsNames.SendStartMessage,new SendStartMessage(_botGetter) },
                {ActionsNames.StartRegistration,new StartRegistration(_repository,_botGetter) },
                {ActionsNames.UnknownCommand,new UnknownCommand(_botGetter) }
            };

            return actions;
        }
    }
}
