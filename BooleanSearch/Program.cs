namespace BooleanSearch
{
    class Program
    {
        private const string ScanResultsPath = @"C:\REPOS\UNI\InformationSearch\NLTK\Results";
        
        static void Main(string[] args)
        {
            var indexes = new IndexBuilder.IndexBuilder()
                .Build(ScanResultsPath);

            var engine = new BooleanSearch.BooleanSearch(indexes);
            var res = engine.Search("пельмен AND браузер AND doordash");
            
            Console.WriteLine(string.Join("\n", res));
        }
    }
}

