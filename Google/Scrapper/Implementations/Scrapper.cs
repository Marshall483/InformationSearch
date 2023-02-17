using System.Net;
using System.Text;
using Google.Helpers;
using Google.Scrapper.Abstractions;
using HtmlAgilityPack;

namespace Google.Scrapper.Implementations;

public class Scrapper : IScrapper
{
    private static WebRequest _request;
    private static WebResponse _response;
    private static Encoding _encode = Encoding.GetEncoding("utf-8");

    public List<string> FindLinks(HtmlDocument doc)
    {
        var linkTags = doc.DocumentNode.Descendants("link");
        
        var linkedPages = doc.DocumentNode.Descendants("a")
            .Select(a => a.GetAttributeValue("href", null))
            .Where(u => !String.IsNullOrEmpty(u) && u.StartsWith("http"));

        return linkedPages.ToList();
    }
    
    public OperationResult<string, Exception> GetHtmlString(string url)
    {
        _request = WebRequest.Create(url);
        _request.Proxy = null;

        try
        {
            _response = _request.GetResponse();
            
            using (StreamReader sReader = new StreamReader(_response.GetResponseStream(), _encode))
            {
                return new SucceededOperationResult<string, Exception>(sReader.ReadToEnd());
            }
        }
        catch(Exception e)
        {
            return new FailedOperationResult<string, Exception>(e);
        }
    }

    public int GetWordsCount(HtmlDocument d)
    {
        var delimiter = new char[] {' '};
        var kelime = 0;
     
        var nodes = d.DocumentNode
            .SelectNodes("//body//text()[not(parent::script)]");

        if (nodes == null)
        {
            return 0;
        }
        
        foreach (string text in nodes.Select(node => node.InnerText))
        {
            var words = text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => Char.IsLetter(s[0]));
            
            var wordCount = words.Count();
            
            if (wordCount > 0)
            {
                kelime += wordCount;
            }
        }
        
        return kelime;
    }

    public string GetTextBody(HtmlDocument doc)
    {
        var s = new StringBuilder();
        
        try
        {
            // Remove script & style nodes
            doc.DocumentNode.Descendants()
                .Where( n => n.Name == "script" || n.Name == "style" )
                .ToList()
                .ForEach(n => n.Remove());
            
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//text()[normalize-space(.) != '']"))
            {
                s.Append(node.InnerText.Trim() + " ");
            }
        }
        catch (Exception)
        {
            // ingore
        }

        return s.ToString();
    }
}