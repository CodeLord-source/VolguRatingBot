using Telegram.Bot;

namespace RatingBot.Bots.Telegram
{
    public interface ITelegramBotGetter
    {
        Task<TelegramBotClient> GetBot();
    }
}