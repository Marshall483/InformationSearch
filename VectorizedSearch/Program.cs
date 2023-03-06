using System;
using VectorizedSearch.TF_IDF;

namespace VectorizedSearch 
{
    internal class Program
    {
        private const string ScanResultsPath = @"C:\REPOS\UNI\InformationSearch\NLTK\Results";
        
        static void Main(string[] args)
        {
            var tfidf = new TfIdf(ScanResultsPath);
            
            tfidf.CalcTF();
            tfidf.CalcIDF();
            tfidf.CalcTF_IDF();
            
            Console.WriteLine("Hello World!");
        }
    }
}