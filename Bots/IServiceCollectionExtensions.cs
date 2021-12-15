using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RatingBot.Bots.Telegram;

namespace RatingBot.Bots
{
    public static class IServiceCollectionExtensions
    {
          
        public static IServiceCollection AddTelegrammBot(this IServiceCollection services, IConfiguration configuration)
        {
            //extension method adding a telegram bot to the application
            var options = CheckTelegramSectionExistence(configuration);
            var tg = new TelegramBotGetter(options);

            services.AddSingleton<ITelegramBotGetter>(tg);

            return services; 
        }

        private static BotOptions CheckTelegramSectionExistence(IConfiguration configuration)
        {
            //checks for the presence of the telegram bot configuration section
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
