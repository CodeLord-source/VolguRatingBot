using System.Diagnostics.CodeAnalysis;

namespace RatingBot.Services.LkVolsuParsing
{
    [ExcludeFromCodeCoverage]
    public class RequestsSettingsNotFoundExeption : Exception
    {
        public RequestsSettingsNotFoundExeption(string message)
            : base(message)
        {

        }

        public RequestsSettingsNotFoundExeption()
        {

        }

        public RequestsSettingsNotFoundExeption(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
