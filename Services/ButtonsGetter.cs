using RatingBot.Models.Buttons;
using Telegram.Bot.Types.ReplyMarkups;

namespace RatingBot.Services
{
    public static class ButtonsGetter
    {
        public static IReplyMarkup? GetRegisteredButtons()
        {
            //returns buttons for registered state
            return new ReplyKeyboardMarkup(

            new List<List<KeyboardButton>>
            {
            new List<KeyboardButton>{new KeyboardButton(ButtonsNames.GET_RATING), new KeyboardButton(ButtonsNames.CHANGE_DATA) }
            });
        }

        public static IReplyMarkup? GetRegistrationButtons()
        {
            //returns registration buttons
            return new ReplyKeyboardMarkup(

            new List<List<KeyboardButton>>
            {
            new List<KeyboardButton>{new KeyboardButton(ButtonsNames.ADD_LOGIN), new KeyboardButton(ButtonsNames.ADD_PASSWORD), new KeyboardButton(ButtonsNames.ADD_SEMESTER), new KeyboardButton(ButtonsNames.COMPLETE_REGISTRATION) }
            });
        }

        public static IReplyMarkup? GetSemesterButtons()
        {
            //returns semester selection buttons
            return new ReplyKeyboardMarkup(

            new List<List<KeyboardButton>>
            {
            new List<KeyboardButton>{new KeyboardButton(ButtonsNames.FIRST_SEMESTER), new KeyboardButton(ButtonsNames.FIFTH_SEMESTER) },
            new List<KeyboardButton>{new KeyboardButton(ButtonsNames.SECOND_SEMESTER), new KeyboardButton(ButtonsNames.SIXTH_SEMESTER) },
            new List<KeyboardButton>{new KeyboardButton(ButtonsNames.THIRD_SEMESTER), new KeyboardButton(ButtonsNames.SEVENTH_SEMESTER) },
            new List<KeyboardButton>{new KeyboardButton(ButtonsNames.FOURTH_SEMESTER), new KeyboardButton(ButtonsNames.EIGHT_SEMESTER) },
            });
        }

        public static IReplyMarkup? GetChangeDataButtons()
        {
            //returns returns data change buttons
            return new ReplyKeyboardMarkup(

            new List<List<KeyboardButton>>
            {
            new List<KeyboardButton>{new KeyboardButton(ButtonsNames.CHANGE_LOGIN), new KeyboardButton(ButtonsNames.CHANGE_PASSWORD), new KeyboardButton(ButtonsNames.CHANGE_SEMESTER), new KeyboardButton(ButtonsNames.SAVE_CHANGES) }
            });
        }
    }
}
