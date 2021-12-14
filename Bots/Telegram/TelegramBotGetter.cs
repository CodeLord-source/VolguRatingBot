using Telegram.Bot;

namespace RatingBot.Bots.Telegram
{
    public class TelegramBotGetter : ITelegramBotGetter
    {
        private readonly BotOptions options;
        private TelegramBotClient client;

        public TelegramBotGetter(BotOptions options)
        {
            this.client = new(options.Token);
            client.SetWebhookAsync(options.WebHook);
        }

        public async Task<TelegramBotClient> GetBot()
        {
            return client;
        }
    }
}
