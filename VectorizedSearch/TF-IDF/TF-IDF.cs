using System.Text;
using Accord.Math.Distances;

namespace VectorizedSearch.TF_IDF;

public class Matrix
{
    private readonly decimal[][] _values;
    private readonly string[] _terms;
    private readonly string[] _docs;

    public string[] GetTerms => _terms;
    public string[] GetDocs => _docs;

    public Matrix(string[] terms, string[] docs)
    {
        _values = new decimal[terms.Length][];

        foreach (var i in Enumerable.Range(0, terms.Length))
        {
            _values[i] = new decimal[docs.Length];
        }
        
        _terms = terms;
        _docs = docs;
    }
    
    public Vector this[string doc]
    {
        get
        {
            var v = new Vector(_terms);
            var i = Array.IndexOf(_docs, doc);

            foreach (var t in _terms)
            {
                v[t] = _values[Array.IndexOf(_terms, t)][i];
            }

            return v;
        }
    }

    public decimal this[string term, string doc]
    {
        get => _values[Array.IndexOf(_terms, term)][Array.IndexOf(_docs, doc)];
        set => _values[Array.IndexOf(_terms, term)][Array.IndexOf(_docs, doc)] = value;
    }

    public void SaveAs(string @as)
    {
        var p = Path.Join(Directory.GetCurrentDirectory(), @as);

        using (var sr = new StreamWriter(File.Create(p), Encoding.UTF8))
        {
            sr.WriteLine(";" + string.Join(";", _docs.Select(Path.GetFileName)));
            
            for (int i = 0; i < _terms.Length; i++)
            {
                sr.Write(_terms[i] + ";");
            
                for (int j = 0; j < _docs.Length; j++)
                {
                    sr.Write(_values[i][j] + ";");
                }
            
                sr.WriteLine();
            }
        }
    }
}

public class Vector
{
    private readonly decimal[] _values;
    private readonly string[] _terms;

    public decimal[] GetValues => _values;

    public Vector(string[] terms)
    {
        _values = new decimal[terms.Length];
        _terms = terms;
    }

    public decimal this[string term]
    {
        get => _values[Array.IndexOf(_terms, term)];
        set => _values[Array.IndexOf(_terms, term)] = value;
    }

    public void SaveAs(string @as)
    {
        var p = Path.Join(Directory.GetCurrentDirectory(), @as);

        using (var sr = new StreamWriter(File.Create(p), Encoding.UTF8))
        {
            sr.WriteLine(string.Join(";", _terms));
            sr.WriteLine(String.Join(";", _values));
        }
    }
}

public class TfIdf
{
    private int _docsCount;

    private string[] _terms;

    private string[] _docs;
    
    private readonly string _sourceFilesDir;
    
    public Matrix TF { get; private set; }
    
    public Vector Vector { get; private set; }
    
    public Matrix TF_IDF { get; private set; }
    
    private Dictionary<string, List<string>> Index { get; set; }

    public TfIdf(string sourceFilesDir)
    {
        _sourceFilesDir = sourceFilesDir;
        
        Init();
    }

    public void SetCustomTerms(string[] newTerms)
    {
        _terms = newTerms;
    }

    public void CalcTF_IDF()
    {
        foreach (var doc in _docs)
        {
            foreach (var term in _terms)
            {
                TF_IDF[term, doc] = TF[term, doc] * Vector[term];
            }
        }
    }

    public void CalcIDF()
    {
        foreach (var term in _terms)
        {
            decimal freq = Index[term].Distinct().Count();
            var val = freq == 0 ? 0 : _docsCount / freq;
            Vector[term] = (decimal)Math.Log((double)val);
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
    
    public Vector QueryIntoVector(string q)
    {
        var v = new Vector(_terms);
        var qs = q.Split(' ');
        var intersect = _terms.Intersect(qs);

        if (!intersect.Any())
        {
            return v;
        }
        
        var gs = intersect.GroupBy(t => t);

        foreach (var g in gs)
        {
            var tf = (decimal)g.ToArray().Length / qs.Length;
            decimal freq = Index[g.Key].Distinct().Count();
            var val = freq == 0 ? 0 : _docsCount / freq;
            var idf = (decimal)Math.Log((double)val);;

            v[g.Key] = tf * idf;
        }

        return v;
    }

    public List<string> Search(Vector v)
    {
        var r = new List<string>();
        
        foreach (var doc in _docs)
        {
            var d = TF_IDF[doc];
            
            var dis = Accord.Math.Distance.Cosine(d.GetValues.Select(v => (double)v).ToArray(),
                v.GetValues.Select(v => (double)v).ToArray());

            if (dis != 1)
            {
                r.Add($"{Path.GetFileName(doc)} - {dis}");
            }
        }

        return r.OrderBy(v => double.Parse(v.Split(" - ")[1])).ToList();
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

        TF = new Matrix(_terms, _docs);
        Vector = new Vector(_terms);
        TF_IDF = new Matrix(_terms, _docs);
    }
}