using HtmlAgilityPack;

namespace RatingBot.Services.LkVolsuParsing
{
    public interface IHtmlDocLoader
    {
        Task<HtmlDocument> LoadAsync(Stream stream);
    }
}
