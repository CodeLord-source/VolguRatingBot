using Appccelerate.StateMachine;
using Appccelerate.StateMachine.AsyncMachine;
using RatingBot.Models.Buttons;
using RatingBot.Models.EventsAndStates;
using RatingBot.Models.StateMachineActions;
using Telegram.Bot.Types;

namespace RatingBot.Services
{
    public class DialogStateMachine
    {
        private readonly AsyncPassiveStateMachine<DialogState, Events> machine;

        public DialogStateMachine(ActionsInitializer init, UserDataCheker cheker)
        {
            var actions = init.Initialize();
            var builder = new StateMachineDefinitionBuilder<DialogState, Events>();

            //Setup state machine 
            builder.WithInitialState(DialogState.NotRegistered);

            #region NOT_REGISTERED

            builder.In(DialogState.NotRegistered)
                .On(Events.Start)
                .If<Update>(cheker.UserIsRegistered)
                .Goto(DialogState.IsRegistered)
                .Execute<Update>(actions[ActionsNames.SendStartMessage].ExecuteAsync)
                .If<Update>(cheker.UserIsNotRegistered)
                .Goto(DialogState.Registering)
                .Execute<Update>(actions[ActionsNames.StartRegistration].ExecuteAsync)

                .On(Events.ChangeData)
                .If<Update>(cheker.UserIsRegistered)
                .Goto(DialogState.Updating)
                .Execute<Update>(actions[ActionsNames.ChangeData].ExecuteAsync)
                .If<Update>(cheker.UserIsNotRegistered)
                .Goto(DialogState.Registering)
                .Execute<Update>(actions[ActionsNames.StartRegistration].ExecuteAsync)

                .On(Events.GetRating)
                .If<Update>(cheker.UserIsRegistered)
                .Execute<Update>(actions[ActionsNames.GetRating].ExecuteAsync)
                .If<Update>(cheker.UserIsNotRegistered)
                .Goto(DialogState.Registering)
                .Execute<Update>(actions[ActionsNames.StartRegistration].ExecuteAsync)

                .On(Events.CompleteRegistration)
                .If<Update>(cheker.UserIsRegistered)
                .Goto(DialogState.IsRegistered)
                .Execute<Update>(actions[ActionsNames.CompleteRegistration].ExecuteAsync)
                .If<Update>(cheker.UserIsNotRegistered)
                .Goto(DialogState.Registering)
                .Execute<Update>(actions[ActionsNames.StartRegistration].ExecuteAsync)

                .On(Events.SaveChanges)
                .If<Update>(cheker.UserIsRegistered)
                .Goto(DialogState.IsRegistered)
                .Execute<Update>(actions[ActionsNames.SaveChanges].ExecuteAsync)
                .If<Update>(cheker.UserIsNotRegistered)
                .Goto(DialogState.Registering)
                .Execute<Update>(actions[ActionsNames.StartRegistration].ExecuteAsync)

                .On(Events.EnterTheLogin)
                .Goto(DialogState.EnteringLogin)
                .Execute<Update>(actions[ActionsNames.LoginEnter].ExecuteAsync)

                .On(Events.EnterThePassword)
                .Goto(DialogState.EnteringPassword)
                .Execute<Update>(actions[ActionsNames.PasswordEnter].ExecuteAsync)

                .On(Events.EnterTheSemester)
                .Goto(DialogState.EnteringSemester)
                .Execute<Update>(actions[ActionsNames.SemesterEnter].ExecuteAsync)

                .On(Events.UserEnter)
                .Execute<Update>(actions[ActionsNames.UnknownCommand].ExecuteAsync);

            #endregion

            #region REGISTRATION

            builder.In(DialogState.Registering)
                .On(Events.EnterTheLogin)
                .Goto(DialogState.EnteringLogin)
                .Execute<Update>(actions[ActionsNames.LoginEnter].ExecuteAsync)

                .On(Events.EnterThePassword)
                .Goto(DialogState.EnteringPassword)
                .Execute<Update>(actions[ActionsNames.PasswordEnter].ExecuteAsync)

                .On(Events.Start)
                .Execute<Update>(actions[ActionsNames.SendStartMessage].ExecuteAsync)

                .On(Events.EnterTheSemester)
                .Goto(DialogState.EnteringSemester)
                .Execute<Update>(actions[ActionsNames.SemesterEnter].ExecuteAsync)

                .On(Events.UserEnter)
                .Execute<Update>(actions[ActionsNames.UnknownCommand].ExecuteAsync)

                .On(Events.CompleteRegistration)
                .If<Update>(cheker.UserIsRegistered)
                .Goto(DialogState.IsRegistered)
                .Execute<Update>(actions[ActionsNames.CompleteRegistration].ExecuteAsync)
                .If<Update>(cheker.UserIsNotRegistered)
                .Execute<Update>(actions[ActionsNames.DataSaveError].ExecuteAsync);

            #endregion

            #region PASSWORD_ENTERING

            builder.In(DialogState.EnteringPassword)
                .On(Events.UserEnter)
                .If<Update>(cheker.UserHasData)
                .Goto(DialogState.Updating)
                .Execute<Update>(actions[ActionsNames.SavePassword].ExecuteAsync)

                .If<Update>(cheker.UserHasNoData)
                .Goto(DialogState.Registering)
                .Execute<Update>(actions[ActionsNames.SavePassword].ExecuteAsync);

            #endregion

            #region LOGIN_ENTERING

            builder.In(DialogState.EnteringLogin)
                .On(Events.UserEnter)
                .If<Update>(cheker.UserHasData)
                .Goto(DialogState.Updating)
                .Execute<Update>(actions[ActionsNames.SaveLogin].ExecuteAsync)

                .If<Update>(cheker.UserHasNoData)
                .Goto(DialogState.Registering)
                .Execute<Update>(actions[ActionsNames.SaveLogin].ExecuteAsync);

            #endregion

            #region SEMESTER_ENTERING

            builder.In(DialogState.EnteringSemester)
                .On(Events.UserEnter)
                .If<Update>(cheker.UserHasData)
                .Goto(DialogState.Updating)
                .Execute<Update>(actions[ActionsNames.SaveSemester].ExecuteAsync)

                .If<Update>(cheker.UserHasNoData)
                .Goto(DialogState.Registering)
                .Execute<Update>(actions[ActionsNames.SaveSemester].ExecuteAsync);

            #endregion

            #region UPDATING

            builder.In(DialogState.Registering)
                .On(Events.EnterTheLogin)
                .Goto(DialogState.EnteringLogin)
                .Execute<Update>(actions[ActionsNames.LoginEnter].ExecuteAsync)

                .On(Events.Start)
                .Execute<Update>(actions[ActionsNames.SendStartMessage].ExecuteAsync)

                .On(Events.EnterThePassword)
                .Goto(DialogState.EnteringPassword)
                .Execute<Update>(actions[ActionsNames.PasswordEnter].ExecuteAsync)

                .On(Events.EnterTheSemester)
                .Goto(DialogState.EnteringSemester)
                .Execute<Update>(actions[ActionsNames.SemesterEnter].ExecuteAsync)

                .On(Events.UserEnter)
                .Execute<Update>(actions[ActionsNames.UnknownCommand].ExecuteAsync)

                .On(Events.SaveChanges)
                .If<Update>(cheker.UserIsRegistered)
                .Goto(DialogState.IsRegistered)
                .Execute<Update>(actions[ActionsNames.SaveChanges].ExecuteAsync)
                .If<Update>(cheker.UserIsNotRegistered)
                .Execute<Update>(actions[ActionsNames.DataSaveError].ExecuteAsync);

            #endregion

            #region REGISTERED

            builder.In(DialogState.IsRegistered)
                .On(Events.GetRating)
                .Execute<Update>(actions[ActionsNames.GetRating].ExecuteAsync)

                .On(Events.ChangeData)
                .Goto(DialogState.Updating)
                .Execute<Update>(actions[ActionsNames.ChangeData].ExecuteAsync)

                .On(Events.Start)
                .Execute<Update>(actions[ActionsNames.SendStartMessage].ExecuteAsync)

                .On(Events.UserEnter)
                .Execute<Update>(actions[ActionsNames.UnknownCommand].ExecuteAsync);

            #endregion

            machine = builder
                 .Build()
                 .CreatePassiveStateMachine();

            machine.Start();
        }

        public async Task ExecuteCommandAsync(Update update)
        {
            var action = (update.Message.Text) switch
            {
                (ButtonsNames.START) => Events.Start,
                (ButtonsNames.ADD_LOGIN) => Events.EnterTheLogin,
                (ButtonsNames.ADD_PASSWORD) => Events.EnterThePassword,
                (ButtonsNames.ADD_SEMESTER) => Events.EnterTheSemester,
                (ButtonsNames.COMPLETE_REGISTRATION) => Events.CompleteRegistration,
                (ButtonsNames.CHANGE_DATA) => Events.ChangeData,
                (ButtonsNames.CHANGE_LOGIN) => Events.EnterTheLogin,
                (ButtonsNames.CHANGE_PASSWORD) => Events.EnterThePassword,
                (ButtonsNames.CHANGE_SEMESTER) => Events.EnterTheSemester,
                (ButtonsNames.SAVE_CHANGES) => Events.SaveChanges,
                (ButtonsNames.GET_RATING) => Events.GetRating,

                _ => Events.UserEnter
            };

            await machine.Fire(action);
        }
    }
}
