using HtmlAgilityPack;
using RatingBot.Models;

namespace RatingBot.Services.Parser
{
    public interface IParser<T> where T : class
    {
        T Parse(HtmlDocument document,int? number);
    }
}
