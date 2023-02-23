using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Google.Scrapper.Abstractions;
using HtmlAgilityPack;

namespace Google.Scrapper.Implementations;

public class Scrapper : IScrapper
{
    private const string CyrillicPattern = @"\p{IsCyrillic}";

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

    public int GetWordsCount(HtmlDocument d)
    {   
        var delimiter = new char[] {' '};
        var total = 0;
        var totalCyrillic = 1;
     
        var nodes = d.DocumentNode
            .SelectNodes("//body//text()[not(parent::script)]");

        if (nodes == null)
        {
            return 0;
        }
        
        foreach (string text in nodes.Select(node => node.InnerText))
        {
            var words = text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => Char.IsLetter(s[0]) && !s.Contains("&nbsp"));

            var asArray = words as string[] ?? words.ToArray();
            
            total += asArray.Count();
            totalCyrillic += asArray.Count(w => Regex.IsMatch(w, CyrillicPattern));
        }

        if ((total / 2) < totalCyrillic)
        {
            return total;
        }
        
        return 0;
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
                if (!node.InnerText.Contains("&nbsp"))
                {
                    s.Append(node.InnerText.Trim() + " ");
                }
            }
        }
        catch (Exception)
        {
            // ingore
        }

        return s.ToString();
    }
}