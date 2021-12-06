using System;
using System.Diagnostics.CodeAnalysis;

namespace RatingBot.Bots.Telegram
{
    [ExcludeFromCodeCoverage]
    public class TelegramBotOptionsNotFoundExeption : Exception
    {
        public TelegramBotOptionsNotFoundExeption(string message)
            : base(message)
        {

        }

        public TelegramBotOptionsNotFoundExeption()
        {

        }

        public TelegramBotOptionsNotFoundExeption(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}