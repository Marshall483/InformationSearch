using Google.Helpers;
using HtmlAgilityPack;

namespace Google.Scrapper.Abstractions;

public interface IScrapper
{
    public OperationResult<string, Exception> GetHtmlString(string url);

    public int GetWordsCount(HtmlDocument d);

    public List<string> FindLinks(HtmlDocument doc);

    public string GetTextBody(HtmlDocument doc);
}