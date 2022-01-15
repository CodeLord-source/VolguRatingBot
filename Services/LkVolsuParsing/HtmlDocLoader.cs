using HtmlAgilityPack;
using RatingBot.Services.LkVolsuParsing;

namespace RatingBot.Services.Parser
{
    public class HtmlDocLoader : IHtmlDocLoader
    {
        public async Task<HtmlDocument> LoadAsync(Stream stream)
        {
            var doc = new HtmlDocument();
            await Task.Run(() => doc.Load(stream));

            return doc;
        }
    }
}
