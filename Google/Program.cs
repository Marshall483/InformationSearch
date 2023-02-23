using Castle.Windsor;
using Google.Сrawler.Abstractions;

namespace Google
{
    class Program
    {
        private static List<string> _targets = new()
        {
           "https://habr.com/ru/all/",
            "https://metanit.com/",
            "https://ficbook.net/",
            "https://4pda.to/",
            "https://rbc.ru/"
        };
        
        static void Main(string[] args)
        {
            var q = new Queue<string>();
            var c = new WindsorContainer();
            
            _targets.ForEach(q.Enqueue);
            c.Install(new ApplicationCastleInstaller());

            var crawler = c.Resolve<ICrawler>();

            crawler.SetResultsDir();
            crawler.Scan(q);
        }
    }
}

