using Google.Scrapper.Abstractions;
using Google.Сrawler.Abstractions;
using HtmlAgilityPack;

namespace Google.Сrawler.Implementations;

public class Crawler : ICrawler
{
    private const string ResDir = "Results";
    
    private const string IndexFileName = "index.txt";

    private const int DesiredWordsCount = 1000;

    private const int NeedToFind = 100;

    private List<string> _visited = new();
    
    private string _targetDir { get; set; }

    public IScrapper Scrapper { get; set; }
    
    public void SetResultsDir(string baseDir = "")
    {
        if (string.IsNullOrEmpty(baseDir))
        {
            baseDir = AppDomain.CurrentDomain.BaseDirectory;
        }

        baseDir = Path.Combine(baseDir, ResDir);
        
        var last = Directory
            .GetDirectories(baseDir)
            .Count();

        _targetDir = Directory.CreateDirectory(
            Path.Combine(
                baseDir, 
                (++last).ToString()))
            .FullName;
    }
    
    public void Scan(Queue<string?> urls)
    {
        var found = 0;
        
        using (var index = new StreamWriter(File.Create(Path.Combine(_targetDir, IndexFileName))))
        {
            while (urls.TryDequeue(out string? url))
            {
                if (found >= NeedToFind)
                {
                    break;
                }

                if (_visited.Contains(url))
                {
                    continue;
                }

                HtmlDocument d = null;
                
                try
                {
                    d = new HtmlWeb().Load(url);
                }
                catch (Exception e)
                {
                    continue;
                }
                
                var links = Scrapper.FindLinks(d);
                var words = Scrapper.GetWordsCount(d);

                if (words >= DesiredWordsCount)
                {
                    found++;
                    index.WriteLine($"{found} -> {url}");

                    using (var txt = new StreamWriter(File.Create(Path.Combine(_targetDir, $"{found.ToString()}.txt"))))
                    {
                        txt.Write(Scrapper.GetTextBody(d));
                    }
                }
                
                _visited.Add(url);
                links.ForEach(urls.Enqueue);
            }
        }
    }
}