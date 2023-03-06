using System.Text;

namespace VectorizedSearch.TF_IDF;

public class TDMatrix
{
    private readonly decimal[][] _values;
    private readonly string[] _terms;
    private readonly string[] _docs;

    public TDMatrix(string[] terms, string[] docs)
    {
        _values = new decimal[terms.Length][];

        foreach (var i in Enumerable.Range(0, terms.Length))
        {
            _values[i] = new decimal[docs.Length];
        }
        
        _terms = terms;
        _docs = docs;
    }
    
    public decimal[] this[string term]
    {
        get => _values[Array.IndexOf(_terms, term)];
    }

    public decimal this[string term, string doc]
    {
        get => _values[Array.IndexOf(_terms, term)][Array.IndexOf(_docs, doc)];
        set => _values[Array.IndexOf(_terms, term)][Array.IndexOf(_docs, doc)] = value;
    }
}

public class IDF
{
    private readonly decimal[] _idFs;
    private readonly string[] _terms;

    public IDF(string[] terms)
    {
        _idFs = new decimal[terms.Length];
        _terms = terms;
    }

    public decimal this[string term]
    {
        get => _idFs[Array.IndexOf(_terms, term)];
        set => _idFs[Array.IndexOf(_terms, term)] = value;
    }
}

public class TfIdf
{
    private int _docsCount;

    private string[] _terms;

    private string[] _docs;
    
    private readonly string _sourceFilesDir;
    
    public TDMatrix TF { get; private set; }
    
    public IDF IDF { get; private set; }
    
    public TDMatrix TF_IDF { get; private set; }
    
    private Dictionary<string, List<string>> Index { get; set; }

    public TfIdf(string sourceFilesDir)
    {
        _sourceFilesDir = sourceFilesDir;
        
        Init();
    }

    public void CalcTF_IDF()
    {
        foreach (var doc in _docs)
        {
            foreach (var term in _terms)
            {
                TF_IDF[term, doc] = TF[term, doc] * IDF[term];
            }
        }
    }

    public void CalcIDF()
    {
        foreach (var term in _terms)
        {
            decimal freq = Index[term].Distinct().Count();
            IDF[term] = freq == 0 ? 0 : _docsCount / freq;
        }
    }

    public void CalcTF()
    {
        foreach (var doc in _docs)
        {
            using (var sr = new StreamReader(
                       new FileStream(doc, FileMode.Open),
                       Encoding.UTF8))
            {
                var words = sr.ReadLine().Split(' ');
                
                foreach (var term in _terms)
                {
                    decimal entries = words.Count(t => t.Equals(term));
                    TF[term, doc] = entries / words.Length;
                }
            }
        }
    }
    
    private void Init()
    {
        Index = new Dictionary<string, List<string>>();
        
        _docs = Directory.GetFiles(_sourceFilesDir);
        _docsCount = _docs.Length;
        
        try
        {
            foreach (var f in _docs)
            {
                using (var sr = new StreamReader(
                           new FileStream(f, FileMode.Open),
                           Encoding.UTF8))
                {
                    foreach (var token in sr.ReadLine().Split(' '))
                    {
                        if (Index.ContainsKey(token))
                        {
                            Index[token].Add(f);
                        }
                        else
                        {
                            Index.Add(token, new List<string> { f });
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        _terms = Index.Keys.ToArray();

        TF = new TDMatrix(_terms, _docs);
        IDF = new IDF(_terms);
        TF_IDF = new TDMatrix(_terms, _docs);
    }
}