namespace BooleanSearch.BooleanSearch;

public class BooleanSearch
{
    private Dictionary<string, List<string>> Indexes;

    public BooleanSearch(Dictionary<string, List<string>> indexes)
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
        var l = new List<string>();

        var parts = what.Split(' ');
        
        var p1 = parts[1];
        var p2 = parts[3];

        var q1 = new Query(parts[0]);
        var q2 = new Query(parts[2]);
        var q3 = new Query(parts[4]);
        
        var tokens = Indexes.Keys;

        if (q1.Not 
                ? !tokens.Contains(q1.Term) 
                : tokens.Contains(q1.Term))
        {
            l.AddRange(Indexes[q1.Term]);
        }

        if (p1.Equals("AND")
            && q2.Not 
                ? !tokens.Contains(q2.Term) 
                : tokens.Contains(q2.Term))
        {
            l = l.Intersect(Indexes[q2.Term]).ToList();
        }
        else if(q2.Not 
                    ? !tokens.Contains(q2.Term) 
                    : tokens.Contains(q2.Term))
        {
            l = l.Union(Indexes[q2.Term]).ToList();
        }

        if (p2.Equals("AND")
            && q3.Not 
                ? !tokens.Contains(q3.Term) 
                : tokens.Contains(q3.Term))
        {
            l = l.Intersect(Indexes[q3.Term]).ToList();
        }
        else if(q3.Not 
                    ? !tokens.Contains(q3.Term) 
                    : tokens.Contains(q3.Term))
        {
            l = l.Union(Indexes[q3.Term]).ToList();
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