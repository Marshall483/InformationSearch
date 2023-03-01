using System.Text;
using Newtonsoft.Json;

namespace BooleanSearch.IndexBuilder;

public class IndexBuilder 
{
    public void Build(string sourceFilesDir, string outputDir)
    {
        var index = new Dictionary<string, string>();
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
                }

                using (var sr = new StreamWriter(File.Create(Path.Combine(outputDir, Path.GetFileName(f).Replace(".txt", ".json"))),
                           Encoding.UTF8))
                {
                    sr.Write(JsonConvert.SerializeObject(index));
                }

                pos = 0;
                index = new Dictionary<string, string>();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}