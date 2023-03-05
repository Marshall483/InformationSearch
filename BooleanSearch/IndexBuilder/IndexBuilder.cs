using System.Text;

namespace BooleanSearch.IndexBuilder;

public class IndexBuilder 
{
    public Dictionary<string, List<string>> Build(string sourceFilesDir)
    {
        var index = new Dictionary<string, List<string>>();
        var files = Directory.GetFiles(sourceFilesDir);
        
        try
        {
            foreach (var f in files)
            {
                using (var sr = new StreamReader(
                           new FileStream(f, FileMode.Open),
                           Encoding.UTF8))
                {
                    foreach (var token in sr.ReadLine().Split(' '))
                    {
                        if (index.ContainsKey(token))
                        {
                            index[token].Add(f);
                        }
                        else
                        {
                            index.Add(token, new List<string> { f });
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        return index;
    }
}