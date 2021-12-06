using Appccelerate.StateMachine;
using Appccelerate.StateMachine.AsyncMachine;
using RatingBot.Bots.Telegram;
using RatingBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using VolguRatingBot.Services.Repository.Interface;

namespace RatingBot.Services
{
    public class DialogStateMachine
    {
        private readonly ILogger<DialogStateMachine> logger;
        private readonly TelegramBotClient client;
        private readonly IRepository repository;
        private readonly AsyncPassiveStateMachine<DialogState, Actions> machine;

        public Update Update { get; set; }

        public DialogStateMachine(ILogger<DialogStateMachine> logger, ITelegramBotGetter getter, IRepository repository)
        {
            this.logger = logger;
            this.client = getter.GetBot().Result;
            this.repository = repository;

            var builder = new StateMachineDefinitionBuilder<DialogState, Actions>();

            builder.WithInitialState(DialogState.NotRegistered);
              
            builder.In(DialogState.NotRegistered)
                .On(Actions.Start)
                .Goto(DialogState.Registration).Execute(Start);

            builder.In(DialogState.Registration)
                .On(Actions.EnterTheLogin)
                .Goto(DialogState.LoginEnter).Execute(LoginEnter);

            builder.In(DialogState.LoginEnter)
                .On(Actions.UserEnter)
                .Goto(DialogState.Registration).Execute(SaveLogin);

            builder.In(DialogState.Registration)
                .On(Actions.EnterThePassword)
                .Goto(DialogState.PasswordEnter).Execute(PasswordEnter);

            builder.In(DialogState.PasswordEnter)
                .On(Actions.UserEnter)
                .Goto(DialogState.Registration).Execute(SavePassword);

            builder.In(DialogState.Registration)
                .On(Actions.EnterTheSemester)
                .Goto(DialogState.SemesterEnter).Execute(SemesterEnter);

            builder.In(DialogState.SemesterEnter)
                .On(Actions.UserEnter)
                .Goto(DialogState.Registration).Execute(SaveSemester);

            builder.In(DialogState.Registration)
                .On(Actions.CompleteRegistration)
                .Goto(DialogState.Registered).Execute(CompleteRegistration);

            builder.In(DialogState.Registered)
                .On(Actions.ToChangeTheData)
                .Goto(DialogState.DataChange).Execute(GoToChangeData);

            builder.In(DialogState.DataChange)
                .On(Actions.SaveChanges)
                .Goto(DialogState.Registered).Execute(SaveChanges);

            builder.In(DialogState.DataChange)
                .On(Actions.ChangeLogin)
                .Goto(DialogState.LoginChange).Execute(LoginEnter);

            builder.In(DialogState.DataChange)
                .On(Actions.ChangePasssword)
                .Goto(DialogState.PasswordChange).Execute(PasswordEnter);

            builder.In(DialogState.DataChange)
                .On(Actions.ChangeSemester)
                .Goto(DialogState.SemesterChange).Execute(SemesterEnter);

            builder.In(DialogState.SemesterChange)
                .On(Actions.UserEnter)
                .Goto(DialogState.DataChange).Execute(SaveChangedSemester);

            builder.In(DialogState.PasswordChange)
                .On(Actions.UserEnter)
                .Goto(DialogState.DataChange).Execute(SaveChangedPassword);

            builder.In(DialogState.LoginChange)
               .On(Actions.UserEnter)
               .Goto(DialogState.DataChange).Execute(SaveChangedLogin);

            builder.In(DialogState.NotRegistered)
                .On(Actions.UserChecking)
                .Goto(DialogState.Registered);

            builder.In(DialogState.Registered)
                .On(Actions.UserChecking)
                .Goto(DialogState.Registration);

            builder.In(DialogState.Registered).On(Actions.GetRating).Execute(GetRating);

            machine = builder
                .Build()
                .CreatePassiveStateMachine();

            machine.Start();
        }

        public enum DialogState
        {
            NotRegistered,
            Registered,
            Registration,
            DataChange,
            LoginEnter,
            LoginChange,
            PasswordChange,
            SemesterChange,
            PasswordEnter,
            SemesterEnter,
            CheckingUserRegistration
        }

        public enum Actions
        {
            Start,
            CompleteRegistration,
            ToChangeTheData,
            SaveChanges,
            EnterThePassword,
            EnterTheLogin,
            EnterTheSemester,
            ChangeLogin,
            ChangePasssword,
            ChangeSemester,
            UserEnter,
            GetRating,
            UserChecking
        }

        public async Task ExecuteCommand(Update update)
        {
            var action = (update.Message.Text) switch
            {
                ("/start") => Actions.Start,

                ("Добавить логин") => Actions.EnterTheLogin,
                ("Добавить пароль") => Actions.EnterThePassword,
                ("Добавить семестр") => Actions.EnterTheSemester,

                ("Завершить регистрацию") => Actions.CompleteRegistration,

                ("Изменить данные") => Actions.ToChangeTheData,
                ("Изменить логин") => Actions.ChangeLogin,
                ("Изменить пароль") => Actions.ChangePasssword,
                ("Изменить семестр") => Actions.ChangeSemester,

                ("Сохранить изменения") => Actions.SaveChanges,

                ("Рейтинг") => Actions.GetRating,

                _ => Actions.UserEnter
            };

            await machine.Fire(action);
        }

        private async Task Start()
        { 
            var student = await repository.GetStudentAsync(Update.Message.Chat.Id);

            if (student==null || student.Pass == null || student.Log == null || student.Semester == null)
            {
                student = new Student()
                {
                    ChatId = Update.Message.Chat.Id
                };

                await repository.AddAsync(student);
                await client.SendTextMessageAsync(Update.Message.Chat.Id,
                    "Привет,я рейтинг бот,чтобы начать работу,тебе нужно зарегистрироваться,для этого тебе нужно ввести логин и пароль от сайта lk.volsu,а так же выбрать семестр.Для этого используй кнопки ниже.",
                    replyMarkup: GetRegistrationButtons());
            }
            else
            {
                await machine.Fire(Actions.UserChecking);
                await client.SendTextMessageAsync(Update.Message.Chat.Id,
                    "Привет,я рейтинг бот,чтобы посмотреть свои баллы,выбери команду рейтинг,если же ты поменял логи и пароль от сайта lk.volsu,выбери команду изменить данные и введи новые логин/пароль.",
                    replyMarkup: GetRegisteredButtons());
            } 
        }

        private async Task SemesterEnter()
        {
            var message = Update.Message;

            await client.SendTextMessageAsync(message.Chat.Id,
                    "Выбери семестр.",
                    replyMarkup: GetSemesterButtons());
        }

        private async Task LoginEnter()
        {
            var message = Update.Message;

            await client.SendTextMessageAsync(message.Chat.Id,
                    "Введите логин.",
                    replyMarkup: new ReplyKeyboardRemove());
        }

        private async Task PasswordEnter()
        {
            var message = Update.Message;

            await client.SendTextMessageAsync(message.Chat.Id,
                    "Введите пароль.",
                    replyMarkup: new ReplyKeyboardRemove());
        }

        private async Task SaveLogin()
        { 
            var student = await repository.GetStudentAsync(Update.Message.Chat.Id);
            student.Log = Update.Message.Text;

            await repository.UpdateAsync(student);
            await client.SendTextMessageAsync(Update.Message.Chat.Id,
                   "Логин успешно добавлен.",
                   replyMarkup: GetRegistrationButtons());
        }

        private async Task SavePassword()
        { 
            var student = await repository.GetStudentAsync(Update.Message.Chat.Id);
            student.Pass = Update.Message.Text;

            await repository.UpdateAsync(student);
            await client.SendTextMessageAsync(Update.Message.Chat.Id,
                   "Пароль успешно добавлен.",
                   replyMarkup: GetRegistrationButtons());
        }

        private async Task SaveSemester()
        { 
            var student = await repository.GetStudentAsync(Update.Message.Chat.Id);
            student.Semester = Convert.ToInt32(Update.Message.Text);

            await repository.UpdateAsync(student);
            await client.SendTextMessageAsync(Update.Message.Chat.Id,
                   "Семестр успешно добавлен.",
                   replyMarkup: GetRegistrationButtons());
        }

        private async Task CompleteRegistration()
        {
            var student = await repository.GetStudentAsync(Update.Message.Chat.Id);

            if (student.Log == null || student.Log.Length < 10 || student.Pass == null || student.Pass.Length < 9 || student.Semester == null)
            {
                await client.SendTextMessageAsync(Update.Message.Chat.Id,
                       "Вы ввели не все данные,пожалуйста,заполните все данные,чтобы начать пользовться ботом.",
                       replyMarkup: GetRegistrationButtons());

                await machine.Fire(Actions.UserChecking);
            }
            else
            {
                await client.SendTextMessageAsync(Update.Message.Chat.Id,
                       "Регистрация завершена.",
                       replyMarkup: GetRegisteredButtons());
            }
        }

        private void GetRating()
        {
            throw new NotImplementedException();
        }

        private async Task GoToChangeData()
        {
            await client.SendTextMessageAsync(Update.Message.Chat.Id,
                   "Выберете одну из комманд.",
                   replyMarkup: GetChangeDataButtons());
        }

        private async Task SaveChangedLogin()
        {
            var student = await repository.GetStudentAsync(Update.Message.Chat.Id);
            student.Log = Update.Message.Text;

            await repository.UpdateAsync(student);
            await client.SendTextMessageAsync(Update.Message.Chat.Id,
                   "Логин успешно добавлен.",
                   replyMarkup: GetChangeDataButtons());
        }

        private async Task SaveChangedPassword()
        {
            var student = await repository.GetStudentAsync(Update.Message.Chat.Id);
            student.Pass = Update.Message.Text;

            await repository.UpdateAsync(student);
            await client.SendTextMessageAsync(Update.Message.Chat.Id,
                   "Пароль успешно добавлен.",
                   replyMarkup: GetChangeDataButtons());
        }

        private async Task SaveChangedSemester()
        {
            var student = await repository.GetStudentAsync(Update.Message.Chat.Id);
            student.Semester = Convert.ToInt32(Update.Message.Text);

            await repository.UpdateAsync(student);
            await client.SendTextMessageAsync(Update.Message.Chat.Id,
                   "Семестр успешно добавлен.",
                   replyMarkup: GetChangeDataButtons());
        }

        private async Task SaveChanges()
        {
            await client.SendTextMessageAsync(Update.Message.Chat.Id,
                   "Изменения сохранены.",
                   replyMarkup: GetRegisteredButtons());
        }

        private static IReplyMarkup? GetRegisteredButtons()
        {
            return new ReplyKeyboardMarkup(

            new List<List<KeyboardButton>>
            {
            new List<KeyboardButton>{new KeyboardButton("Рейтинг"), new KeyboardButton("Изменить данные")}
            });
        }

        private static IReplyMarkup? GetRegistrationButtons()
        {
            return new ReplyKeyboardMarkup(

            new List<List<KeyboardButton>>
            {
            new List<KeyboardButton>{new KeyboardButton("Добавить логин"), new KeyboardButton("Добавить пароль"), new KeyboardButton("Добавить семестр"), new KeyboardButton("Завершить регистрацию") }
            });
        }

        private static IReplyMarkup? GetSemesterButtons()
        {
            return new ReplyKeyboardMarkup(

            new List<List<KeyboardButton>>
            {
            new List<KeyboardButton>{new KeyboardButton("1"), new KeyboardButton("2")},
            new List<KeyboardButton>{new KeyboardButton("3"), new KeyboardButton("4")},
            new List<KeyboardButton>{new KeyboardButton("5"), new KeyboardButton("6")},
            new List<KeyboardButton>{new KeyboardButton("7"), new KeyboardButton("8")},
            });
        }

        private static IReplyMarkup? GetChangeDataButtons()
        {
            return new ReplyKeyboardMarkup(

            new List<List<KeyboardButton>>
            {
            new List<KeyboardButton>{new KeyboardButton("Изменить логин"), new KeyboardButton("Изменить пароль"), new KeyboardButton("Изменить семестр"), new KeyboardButton("Сохранить изменения") }
            });
        }
    }
}
