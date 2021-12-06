using RatingBot.Bots.Telegram;

namespace RatingBot.Bots
{
    public static class IServiceCollectionExtensions
    {
          
        public static IServiceCollection AddTelegrammBot(this IServiceCollection services, IConfiguration configuration)
        {
            var options = CheckTelegramSectionExistence(configuration);
            var tg = new TelegramBotGetter(options);

            services.AddSingleton<ITelegramBotGetter>(tg);

            return services; 
        }

        private static BotOptions CheckTelegramSectionExistence(IConfiguration configuration)
        {
            var options = configuration.GetSection(BotOptions.SECTION_NAME).Get<BotOptions>();

            if (options == null)
            {
                throw new TelegramBotOptionsNotFoundExeption("Bot configuration section not found.");
            }
            else
            {
                return options;
            }
        }
    }
}
