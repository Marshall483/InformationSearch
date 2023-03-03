using System.Text;
using Newtonsoft.Json;

namespace BooleanSearch.IndexBuilder;

public record struct IndexDto(string path, Dictionary<string, string> Index);

public class IndexBuilder 
{
    public List<IndexDto> Build(string sourceFilesDir, string outputDir)
    {
        var res = new List<IndexDto>();
        var pos = 0;

        try
        {
            var files = Directory.GetFiles(sourceFilesDir);

            foreach (var f in files)
            {
                using (var sr = new StreamReader(
                           new FileStream(f, FileMode.Open),
                           Encoding.UTF8))
                {
                    var index = new Dictionary<string, string>();
                    
                    foreach (var token in sr.ReadLine().Split(' '))
                    {
                        pos++;
                        var newPos = $" (1, {pos})";

                        if (index.ContainsKey(token))
                        {
                            index[token] += newPos;
                        }
                        else
                        {
                            index.Add(token, newPos);
                        }
                    }
                    
                    res.Add(new IndexDto(f, index));
                }

                /*using (var sr = new StreamWriter(File.Create(Path.Combine(outputDir, Path.GetFileName(f).Replace(".txt", ".json")))))
                {
                    sr.Write(JsonConvert.SerializeObject(index));
                }*/
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<IndexDto>();
        }
        
        return res;
    }
}