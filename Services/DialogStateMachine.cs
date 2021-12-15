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
        private Update update;

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
            GotoRegistered,
            GotoDataChange,
            StartRegistration,
            GotoRegistration
        }

        public DialogStateMachine(ILogger<DialogStateMachine> logger, ITelegramBotGetter getter, IRepository repository)
        {
            //Initial:logger,TelegramBotClient,StudentRepository,state machine builder 
            this.logger = logger;
            this.client = getter.GetBot().Result;
            this.repository = repository;
            var builder = new StateMachineDefinitionBuilder<DialogState, Actions>();
            //

            builder.WithInitialState(DialogState.NotRegistered);//setting the initial state

            //Setting up a state machine 
            //1:for not registered state
            builder.In(DialogState.NotRegistered).On(Actions.GotoRegistered).Goto(DialogState.Registered);
            builder.In(DialogState.NotRegistered).On(Actions.GotoRegistration).Goto(DialogState.Registration);
            builder.In(DialogState.NotRegistered).On(Actions.GotoDataChange).Goto(DialogState.DataChange);
            builder.In(DialogState.NotRegistered).On(Actions.StartRegistration).Goto(DialogState.Registration).Execute(StartRegistration);
            builder.In(DialogState.NotRegistered).On(Actions.Start).Execute(() => ChekUserRegistrationState(Actions.Start));
            builder.In(DialogState.NotRegistered).On(Actions.UserEnter).Execute(() => ChekUserRegistrationState(Actions.UserEnter));
            builder.In(DialogState.NotRegistered).On(Actions.GetRating).Execute(() => ChekUserRegistrationState(Actions.GetRating));
            builder.In(DialogState.NotRegistered).On(Actions.ToChangeTheData).Execute(() => ChekUserRegistrationState(Actions.ToChangeTheData));
            builder.In(DialogState.NotRegistered).On(Actions.EnterTheLogin).Execute(() => ChekUserRegistrationState(Actions.EnterTheLogin));
            builder.In(DialogState.NotRegistered).On(Actions.EnterThePassword).Execute(() => ChekUserRegistrationState(Actions.EnterThePassword));
            builder.In(DialogState.NotRegistered).On(Actions.EnterTheSemester).Execute(() => ChekUserRegistrationState(Actions.EnterTheSemester));
            builder.In(DialogState.NotRegistered).On(Actions.CompleteRegistration).Execute(() => ChekUserRegistrationState(Actions.CompleteRegistration));
            builder.In(DialogState.NotRegistered).On(Actions.SaveChanges).Execute(() => ChekUserRegistrationState(Actions.SaveChanges));
            builder.In(DialogState.NotRegistered).On(Actions.ChangeLogin).Execute(() => ChekUserRegistrationState(Actions.ChangeLogin));
            builder.In(DialogState.NotRegistered).On(Actions.ChangePasssword).Execute(() => ChekUserRegistrationState(Actions.ChangePasssword));
            builder.In(DialogState.NotRegistered).On(Actions.ChangeSemester).Execute(() => ChekUserRegistrationState(Actions.ChangeSemester));
            //

            //2:for registration state
            builder.In(DialogState.Registration).On(Actions.EnterTheLogin).Goto(DialogState.LoginEnter).Execute(LoginEnter);
            builder.In(DialogState.Registration).On(Actions.EnterThePassword).Goto(DialogState.PasswordEnter).Execute(PasswordEnter);
            builder.In(DialogState.Registration).On(Actions.EnterTheSemester).Goto(DialogState.SemesterEnter).Execute(SemesterEnter);
            builder.In(DialogState.Registration).On(Actions.CompleteRegistration).Execute(CompleteRegistration);
            builder.In(DialogState.Registration).On(Actions.UserEnter).Execute(SendErrorMessage);
            builder.In(DialogState.Registration).On(Actions.Start).Execute(StartRegistration);
            builder.In(DialogState.Registration).On(Actions.GotoRegistered).Goto(DialogState.Registered);
            //

            //3:for registratered state 
            builder.In(DialogState.Registered).On(Actions.ToChangeTheData).Goto(DialogState.DataChange).Execute(GoToChangeData);
            builder.In(DialogState.Registered).On(Actions.GetRating).Execute(GetRating);
            builder.In(DialogState.Registered).On(Actions.UserEnter).Execute(SendErrorMessage);
            builder.In(DialogState.Registered).On(Actions.Start).Execute(GetButtonsForRegistered);
            builder.In(DialogState.Registered).On(Actions.GotoRegistration).Goto(DialogState.Registration);
            //

            //4:for DataChange state
            builder.In(DialogState.DataChange).On(Actions.ChangeLogin).Goto(DialogState.LoginChange).Execute(LoginEnter);
            builder.In(DialogState.DataChange).On(Actions.ChangePasssword).Goto(DialogState.PasswordChange).Execute(PasswordEnter);
            builder.In(DialogState.DataChange).On(Actions.ChangeSemester).Goto(DialogState.SemesterChange).Execute(SemesterEnter);
            builder.In(DialogState.DataChange).On(Actions.SaveChanges).Execute(SaveChanges);
            builder.In(DialogState.DataChange).On(Actions.UserEnter).Execute(SendErrorMessage);
            builder.In(DialogState.DataChange).On(Actions.Start).Execute(GoToChangeData);
            builder.In(DialogState.DataChange).On(Actions.GotoRegistered).Goto(DialogState.Registered);
            //

            //5:for login enter state
            builder.In(DialogState.LoginEnter).On(Actions.UserEnter).Execute(SaveLogin);
            builder.In(DialogState.LoginEnter).On(Actions.GotoRegistration).Goto(DialogState.Registration);
            //

            //6:for password enter state
            builder.In(DialogState.PasswordEnter).On(Actions.UserEnter).Execute(SavePassword);
            builder.In(DialogState.PasswordEnter).On(Actions.GotoRegistration).Goto(DialogState.Registration);
            //

            //7:for semester enter state
            builder.In(DialogState.SemesterEnter).On(Actions.UserEnter).Execute(SaveSemester);
            builder.In(DialogState.SemesterEnter).On(Actions.GotoRegistration).Goto(DialogState.Registration);
            //

            //8:for login change state
            builder.In(DialogState.LoginChange).On(Actions.UserEnter).Execute(SaveChangedLogin);
            builder.In(DialogState.LoginChange).On(Actions.GotoDataChange).Goto(DialogState.DataChange);
            //

            //9:for password change state
            builder.In(DialogState.PasswordChange).On(Actions.UserEnter).Execute(SaveChangedPassword);
            builder.In(DialogState.PasswordChange).On(Actions.GotoDataChange).Goto(DialogState.DataChange);
            //

            //10:for semester change state
            builder.In(DialogState.SemesterChange).On(Actions.UserEnter).Execute(SaveChangedSemester);
            builder.In(DialogState.SemesterChange).On(Actions.GotoDataChange).Goto(DialogState.DataChange);
            //

            //

            //initial state machine
            machine = builder
                 .Build()
                 .CreatePassiveStateMachine();
            //

            //start state machine
            machine.Start();
            //
        }

        public async Task ExecuteCommand(Update upd)
        {
            this.update = upd;

            var action = (upd.Message.Text) switch
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

            logger.LogInformation($"Вызвано действие {action}", LogLevel.Critical);

            await machine.Fire(action);
        }

        private async void ChekUserRegistrationState(Actions action)
        {
            var chatId = update.Message.Chat.Id;
            var user = await repository.GetStudentAsync(chatId);

            if (user == null || user.Pass == null || user.Log == null || user.Semester == null && (action == Actions.Start || action == Actions.UserEnter))
            {
                await machine.Fire(Actions.StartRegistration);

                return;
            }

            if (user != null && user.Pass != null && user.Log != null && user.Semester != null && action == Actions.GetRating)
            {
                await machine.Fire(Actions.GotoRegistered);
                await GetRating();

                return;
            }

            if (user != null && user.Pass != null && user.Log != null && user.Semester != null && action == Actions.GotoDataChange)
            {
                await machine.Fire(action);

                return;
            }

            if (user != null && user.Pass != null && user.Log != null && user.Semester != null && action == Actions.ChangeLogin)
            {
                await machine.Fire(Actions.GotoDataChange);
                await machine.Fire(action);

                return;
            }

            if (user != null && user.Pass != null && user.Log != null && user.Semester != null && action == Actions.ChangePasssword)
            {
                await machine.Fire(Actions.GotoDataChange);
                await machine.Fire(action);

                return;
            }

            if (user != null && user.Pass != null && user.Log != null && user.Semester != null && action == Actions.ChangeSemester)
            {
                await machine.Fire(Actions.GotoDataChange);
                await machine.Fire(action);

                return;
            }

            if (user != null && user.Pass != null && user.Log != null && user.Semester != null && action == Actions.SaveChanges)
            {
                await machine.Fire(Actions.GotoDataChange);
                await machine.Fire(action);

                return;
            }


            if (user != null && user.Pass != null && user.Log != null && user.Semester != null && action == Actions.CompleteRegistration)
            {
                await machine.Fire(Actions.GotoRegistration);
                await machine.Fire(action);

                return;
            }

            if (user != null && user.Log == null && action == Actions.EnterTheLogin)
            {
                await machine.Fire(Actions.GotoRegistration);
                await machine.Fire(action);

                return;
            }

            if (user != null && user.Pass == null && action == Actions.EnterThePassword)
            {
                await machine.Fire(Actions.GotoRegistration);
                await machine.Fire(action);

                return;
            }

            if (user != null && user.Pass == null && action == Actions.EnterTheSemester)
            {
                await machine.Fire(Actions.GotoRegistration);
                await machine.Fire(action);

                return;
            }

            return;
        }

        private async Task StartRegistration()
        {
            var chatId = update.Message.Chat.Id;
            var student = new Student();

            student.ChatId=chatId;
            
            await repository.AddAsync(student);
            await client.SendTextMessageAsync(chatId,
                "Привет,я рейтинг бот,для начала тебе нужно зарегистрироваться,т.е ввести данные от сайта lk.volsu и выбрать семестр,чтобы я мог отправить тебе твои баллы.Пожалуйста,выбери одну из комманд ниже.",
                replyMarkup: GetRegistrationButtons());
        }

        private async Task SendErrorMessage()
        {
            var chatId = update.Message.Chat.Id;

            await client.SendTextMessageAsync(chatId,
                "Вы ввели некорректные данные,пожалуйства выберите ондну из комманд ниже.");
        }

        private async Task SemesterEnter()
        {
            await client.SendTextMessageAsync(update.Message.Chat.Id,
                    "Выбери семестр.",
                    replyMarkup: GetSemesterButtons());

            logger.LogCritical($"Выплнено semester enter");
        }

        private async Task LoginEnter()
        {
            var chatId = update.Message.Chat.Id;

            await client.SendTextMessageAsync(chatId,
                    "Введите логин.",
                    replyMarkup: new ReplyKeyboardRemove());

            logger.LogCritical($"Выплнено login enter");
        }

        private async Task PasswordEnter()
        {
            await client.SendTextMessageAsync(update.Message.Chat.Id,
                    "Введите пароль.",
                    replyMarkup: new ReplyKeyboardRemove());

            logger.LogCritical($"Выплнено password enter");
        }

        private async Task SaveLogin()
        {
            var student = await repository.GetStudentAsync(update.Message.Chat.Id);
            student.Log = update.Message.Text;

            await repository.UpdateAsync(student);
            await client.SendTextMessageAsync(update.Message.Chat.Id,
                   "Логин успешно добавлен.",
                   replyMarkup: GetRegistrationButtons());
            await machine.Fire(Actions.GotoRegistration);

            logger.LogCritical($"Выплнено save login");
        }

        private async Task SavePassword()
        {
            var student = await repository.GetStudentAsync(update.Message.Chat.Id);
            student.Pass = update.Message.Text;

            await repository.UpdateAsync(student);
            await client.SendTextMessageAsync(update.Message.Chat.Id,
                   "Пароль успешно добавлен.",
                   replyMarkup: GetRegistrationButtons());
            await machine.Fire(Actions.GotoRegistration);

            logger.LogCritical($"Выплнено save password");
        }

        private async Task SaveSemester()
        {
            var student = await repository.GetStudentAsync(update.Message.Chat.Id);
            student.Semester = Convert.ToInt32(update.Message.Text);

            await repository.UpdateAsync(student);
            await client.SendTextMessageAsync(update.Message.Chat.Id,
                   "Семестр успешно добавлен.",
                   replyMarkup: GetRegistrationButtons());
            await machine.Fire(Actions.GotoRegistration);

            logger.LogCritical($"Выплнено save semester");
        }

        private async Task CompleteRegistration()
        {
            var chatid = update.Message.Chat.Id;

            var student = await repository.GetStudentAsync(chatid);

            if (student.Log != null && student.Pass != null && student.Semester != null)
            {
                await machine.Fire(Actions.GotoRegistered);
                await client.SendTextMessageAsync(chatid,
                          "Регистрация завершена.",
                          replyMarkup: GetRegisteredButtons());

                logger.LogInformation($"Выплнено complete registration");
            }
            else
            {
                await client.SendTextMessageAsync(chatid,
                       "Вы ввели не все данные,заполните недостающее,чтобы продолжить.",
                       replyMarkup: GetRegistrationButtons());

                logger.LogInformation($"Не выплнено complete registration");
            }
        }

        private async Task GetRating()
        {
            var chatId = update.Message.Chat.Id;

            await client.SendTextMessageAsync(chatId,
               $"HHH",
               replyMarkup: GetRegisteredButtons());

            logger.LogCritical($"Выплнено get rating");
        }

        private async Task GoToChangeData()
        {
            var chatId = update.Message.Chat.Id;

            await client.SendTextMessageAsync(chatId,
                   "Выберете одну из комманд.",
                   replyMarkup: GetChangeDataButtons());

            logger.LogCritical($"Выплнено go to change data");
        }

        private async Task SaveChangedLogin()
        {
            var chatId = update.Message.Chat.Id;
            var student = await repository.GetStudentAsync(chatId);

            student.Log = update.Message.Text;

            await repository.UpdateAsync(student);
            await client.SendTextMessageAsync(chatId,
                   "Логин успешно изменен.",
                   replyMarkup: GetChangeDataButtons());
            await machine.Fire(Actions.GotoDataChange);

            logger.LogCritical($"Выплнено save changed login");
        }

        private async Task SaveChangedPassword()
        {
            var chatId = update.Message.Chat.Id;
            var student = await repository.GetStudentAsync(chatId);

            student.Pass = update.Message.Text;

            await repository.UpdateAsync(student);
            await client.SendTextMessageAsync(chatId,
                   "Пароль успешно изменен.",
                   replyMarkup: GetChangeDataButtons());
            await machine.Fire(Actions.GotoDataChange);

            logger.LogCritical($"Выплнено save changed password");
        }

        private async Task SaveChangedSemester()
        {
            var chatId = update.Message.Chat.Id;
            var student = await repository.GetStudentAsync(chatId);

            student.Semester = Convert.ToInt32(update.Message.Text);

            await repository.UpdateAsync(student);
            await client.SendTextMessageAsync(chatId,
                   "Семестр успешно изменен.",
                   replyMarkup: GetChangeDataButtons());
            await machine.Fire(Actions.GotoDataChange);

            logger.LogCritical($"Выплнено save changed semester");
        }

        private async Task SaveChanges()
        {
            await machine.Fire(Actions.GotoRegistered);
            var chatId = update.Message.Chat.Id;

            await client.SendTextMessageAsync(chatId,
                   "Изменения сохранены.",
                   replyMarkup: GetRegisteredButtons());

            logger.LogCritical($"Выплнено save changes ");
        }

        private async Task GetButtonsForRegistered()
        {
            var chatId = update.Message.Chat.Id;

            await client.SendTextMessageAsync(chatId,
                          "Выбери одну из комманд.",
                          replyMarkup: GetRegisteredButtons());

            logger.LogCritical($"Выплнено complete registration");
        }

        private static IReplyMarkup? GetRegisteredButtons()
        {
            return new ReplyKeyboardMarkup(

            new List<List<KeyboardButton>>
            {
            new List<KeyboardButton>{new KeyboardButton(ButtonNames.GET_RATING), new KeyboardButton(ButtonNames.CHANGE_DATA) }
            });


        }

        private static IReplyMarkup? GetRegistrationButtons()
        {
            return new ReplyKeyboardMarkup(

            new List<List<KeyboardButton>>
            {
            new List<KeyboardButton>{new KeyboardButton(ButtonNames.ADD_LOGIN), new KeyboardButton(ButtonNames.ADD_PASSWORD), new KeyboardButton(ButtonNames.ADD_SEMESTER), new KeyboardButton(ButtonNames.COMPLETE_REGISTRATION) }
            });
        }

        private static IReplyMarkup? GetSemesterButtons()
        {
            return new ReplyKeyboardMarkup(

            new List<List<KeyboardButton>>
            {
            new List<KeyboardButton>{new KeyboardButton(ButtonNames.FIRST_SEMESTER), new KeyboardButton(ButtonNames.FIFTH_SEMESTER) },
            new List<KeyboardButton>{new KeyboardButton(ButtonNames.SECOND_SEMESTER), new KeyboardButton(ButtonNames.SIXTH_SEMESTER) },
            new List<KeyboardButton>{new KeyboardButton(ButtonNames.THIRD_SEMESTER), new KeyboardButton(ButtonNames.SEVENTH_SEMESTER) },
            new List<KeyboardButton>{new KeyboardButton(ButtonNames.FOURTH_SEMESTER), new KeyboardButton(ButtonNames.EIGHT_SEMESTER) },
            });
        }

        private static IReplyMarkup? GetChangeDataButtons()
        {
            return new ReplyKeyboardMarkup(

            new List<List<KeyboardButton>>
            {
            new List<KeyboardButton>{new KeyboardButton(ButtonNames.CHANGE_LOGIN), new KeyboardButton(ButtonNames.CHANGE_PASSWORD), new KeyboardButton(ButtonNames.CHANGE_SEMESTER), new KeyboardButton(ButtonNames.SAVE_CHANGES) }
            });
        }
    }
}
