using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;

namespace BooleanSearch.BooleanSearch;

public class BooleanSearch
{
    private List<string> IndexesPath { get; set; }

    private List<Dictionary<string, string>> Indexes;
        

    public BooleanSearch(List<Dictionary<string, string>> indexes)
    {
        Indexes = indexes;
    }
    /// <summary>
    /// Executes search across all files in 'where'
    /// </summary>
    /// <param name="what">Search string. Assumed to be in format 'xxx AND yyy OR !zzz'</param>
    /// <returns>Documents which satisfy given search string</returns>
    public List<string> Search(string what)
    {
        var i = 0;
        var l = new List<string>();

        var parts = what.Split(' ');
        
        var p1 = parts[1];
        var p2 = parts[3];

        var q1 = new Query(parts[0]);
        var q2 = new Query(parts[2]);
        var q3 = new Query(parts[4]);

        foreach (var index in Indexes)
        {
            i++;
            bool res;
            var tokens = index.Keys.ToList();
            
            res = q1.Not ? !tokens.Contains(q1.Term) : tokens.Contains(q1.Term);

            if (p1.Equals("AND"))
            {
                res = res && q2.Not ? !tokens.Contains(q2.Term) : tokens.Contains(q2.Term);
            }
            else
            {
                res = res || q2.Not ? !tokens.Contains(q2.Term) : tokens.Contains(q2.Term);
            }

            if (p2.Equals("AND"))
            {
                res = res && q3.Not ? !tokens.Contains(q3.Term) : tokens.Contains(q3.Term);
            }
            else
            {
                res = res || q3.Not ? !tokens.Contains(q3.Term) : tokens.Contains(q3.Term);
            }

            if (res)
            {
                l.Add($"{i}.txt");
            }
        }

        return l;
    }
}

public class Query
{
    public bool Not { get; set; }
    public string Term { get; set; }

    public Query(string q)
    {
        Not = q.StartsWith('!');
        Term = q;
    }
}