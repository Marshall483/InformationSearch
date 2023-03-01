using Castle.Windsor;

namespace BooleanSearch
{
    // new StreamReader(new FileStream(@"C:\REPOS\UNI\InformationSearch\NLTK\Results\1.txt", FileMode.Open), Encoding.UTF8).ReadLine()
    class Program
    {
        private const string IndexFolderName = "Index";
        private const string ScanResultsPath = @"C:\REPOS\UNI\InformationSearch\NLTK\Results";
        
        static void Main(string[] args)
        {
            new IndexBuilder.IndexBuilder()
                .Build(
                    ScanResultsPath, 
                    Path.Combine(Directory.GetCurrentDirectory(), IndexFolderName));
        }
    }
}

