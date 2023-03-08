using System;
using VectorizedSearch.TF_IDF;

namespace VectorizedSearch 
{
    internal class Program
    {
        private const string ScanResultsPath = @"C:\REPOS\UNI\InformationSearch\NLTK\Results";
        private const string SearchString = "пельмен браузер doordash";
        
        static void Main(string[] args)
        {
            var tfidf = new TfIdf(ScanResultsPath);
            
            tfidf.CalcTF();
            tfidf.CalcIDF();
            tfidf.CalcTF_IDF();

            /*tfidf.TF.SaveAs("tf.csv");
            tfidf.Vector.SaveAs("idf.csv");
            tfidf.TF_IDF.SaveAs("tf_idf.csv");*/

            var res = tfidf.Search(
                tfidf.QueryIntoVector(SearchString));

            foreach (var re in res)
            {
                Console.WriteLine(re);
            }
        }
    }
}